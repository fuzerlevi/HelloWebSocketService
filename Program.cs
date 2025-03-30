
using WebSocket_E14KQR;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HelloEndpoint, CinemaEndpoint>();
var app = builder.Build();
app.UseStaticFiles();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.Run();