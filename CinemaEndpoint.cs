using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebSocket_E14KQR
{
    public class CinemaEndpoint : HelloEndpoint
    {
        private static int Rows = 0;
        private static int Columns = 0;
        private static SeatStatus[,] Seats;
        private static Dictionary<string, (int row, int col)> Locks = new();
        private static List<WebSocket> Sockets = new();
        private static int LockCounter = 0;

        private enum SeatStatus { Free, Locked, Reserved }

        public override async Task Open(WebSocket socket)
        {
            Sockets.Add(socket);
            Console.WriteLine("Client connected.");
        }

        public override async Task Close(WebSocket socket)
        {
            Sockets.Remove(socket);
            Console.WriteLine("Client disconnected.");
        }

        public override async Task<string> Message(WebSocket socket, string message)
        {
            try
            {
                var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;
                var type = root.GetProperty("type").GetString();

                switch (type)
                {
                    case "initRoom":
                        int r = root.GetProperty("rows").GetInt32();
                        int c = root.GetProperty("columns").GetInt32();
                        if (r <= 0 || c <= 0)
                            return Error("Invalid room size.");
                        Rows = r;
                        Columns = c;
                        Seats = new SeatStatus[Rows, Columns];
                        return JsonSerializer.Serialize(new { type = "roomSize", rows = Rows, columns = Columns });

                    case "getRoomSize":
                        return JsonSerializer.Serialize(new { type = "roomSize", rows = Rows, columns = Columns });

                    case "updateSeats":
                        for (int i = 0; i < Rows; i++)
                            for (int j = 0; j < Columns; j++)
                                await BroadcastSeat(i, j);
                        return null;

                    case "lockSeat":
                        int lockRow = root.GetProperty("row").GetInt32() - 1;
                        int lockCol = root.GetProperty("column").GetInt32() - 1;
                        if (!InBounds(lockRow, lockCol)) return Error("Invalid seat position.");
                        if (Seats[lockRow, lockCol] != SeatStatus.Free) return Error("Seat is not free.");
                        Seats[lockRow, lockCol] = SeatStatus.Locked;
                        string lockId = $"lock{++LockCounter}";
                        Locks[lockId] = (lockRow, lockCol);
                        await BroadcastSeat(lockRow, lockCol);
                        return JsonSerializer.Serialize(new { type = "lockResult", lockId });

                    case "unlockSeat":
                        string unlockId = root.GetProperty("lockId").GetString();
                        if (!Locks.ContainsKey(unlockId)) return Error("Invalid lock ID.");
                        (int ur, int uc) = Locks[unlockId];
                        Seats[ur, uc] = SeatStatus.Free;
                        Locks.Remove(unlockId);
                        await BroadcastSeat(ur, uc);
                        return null;

                    case "reserveSeat":
                        string resId = root.GetProperty("lockId").GetString();
                        if (!Locks.ContainsKey(resId)) return Error("Invalid lock ID.");
                        (int rr, int rc) = Locks[resId];
                        Seats[rr, rc] = SeatStatus.Reserved;
                        Locks.Remove(resId);
                        await BroadcastSeat(rr, rc);
                        return null;

                    default:
                        return Error("Unknown message type.");
                }
            }
            catch (Exception ex)
            {
                return Error("Server error: " + ex.Message);
            }
        }

        private async Task BroadcastSeat(int row, int col)
        {
            var msg = JsonSerializer.Serialize(new
            {
                type = "seatStatus",
                row = row + 1,
                column = col + 1,
                status = Seats[row, col].ToString().ToLower()
            });

            foreach (var s in Sockets)
                if (s.State == WebSocketState.Open)
                    await StringEncoder.SendAsync(s, msg);
        }

        private static string Error(string message)
        {
            return JsonSerializer.Serialize(new { type = "error", message });
        }

        private static bool InBounds(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }
    }
}