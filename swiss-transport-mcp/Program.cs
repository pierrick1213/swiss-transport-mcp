using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ =>
{
    var client = new HttpClient() { BaseAddress = new Uri("http://transport.opendata.ch/v1/") };
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("swiss-transport-mcp", "1.0"));
    return client;
});

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<TransportdataTools>();

var app = builder.Build();
app.MapMcp();
app.UseHttpsRedirection();

app.Run();
