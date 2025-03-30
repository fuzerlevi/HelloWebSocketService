using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HelloWebSocketService
{
    public class HelloEndpoint
    {
        public async Task Open(WebSocket socket)
        {
            Console.WriteLine("WebSocket opened.");
        }
        public async Task Close(WebSocket socket)
        {
            Console.WriteLine("WebSocket closed.");
        }
        public async Task Error(WebSocket socket, Exception ex)
        {
            Console.WriteLine("WebSocket error: " + ex.Message);
        }
        public async Task<string> Message(WebSocket socket, string message)
        {
            Console.WriteLine($"WebSocket message: {message}");
            return $"Hello: {message}";
        }
    }
}
