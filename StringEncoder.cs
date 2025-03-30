using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWebSocketService
{
    public class StringEncoder
    {
        public static async Task<(WebSocketReceiveResult result, string? message)>
       ReceiveAsync(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(buffer: new
           ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                return (result, text);
            }
            return (result, null);
        }
        public static async Task SendAsync(WebSocket socket, string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0,
           message.Length);
            await socket.SendAsync(buffer: buffer,
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
        }
    }
}
