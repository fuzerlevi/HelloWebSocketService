
using HelloWebSocketService;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HelloEndpoint>();
var app = builder.Build();
app.UseStaticFiles();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.Run();