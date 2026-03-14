using CurrencyUpdater;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("cbr", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<CurrencyWorker>();

var host = builder.Build();
host.Run();
