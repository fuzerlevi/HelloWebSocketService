using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebSocket_E14KQR
{
    public class HelloEndpoint
    {
        public virtual async Task Open(WebSocket socket)
        {
            Console.WriteLine("WebSocket opened.");
        }
        public virtual async Task Close(WebSocket socket)
        {
            Console.WriteLine("WebSocket closed.");
        }
        public virtual async Task Error(WebSocket socket, Exception ex)
        {
            Console.WriteLine("WebSocket error: " + ex.Message);
        }
        public virtual async Task<string> Message(WebSocket socket, string message)
        {
            Console.WriteLine($"WebSocket message: {message}");
            return $"Hello: {message}";
        }
    }
}
