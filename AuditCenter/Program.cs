using System.Net;
using AuditCenter.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 5006, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps();
    });

    // gRPC - http3
    // https://devblogs.microsoft.com/dotnet/http-3-support-in-dotnet-6/
    // options.Listen(IPAddress.Any, 5006, listenOptions =>
    // {
    //     listenOptions.Protocols = HttpProtocols.Http3;
    //     listenOptions.UseHttps();
    // });
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<AuditService>();
app.MapGet("/",
    () =>
        "AuditService is ready to serve you!!! What do you want from me?");

app.Run();
