var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<TransportdataTools>();

var app = builder.Build();
app.MapMcp();
app.UseHttpsRedirection();

app.Run();
