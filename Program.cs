using lifecheck_client;

var builder = Host.CreateDefaultBuilder(args);

builder.UseWindowsService();
builder.ConfigureServices((hostContext, services) => {
	services.AddHostedService<Worker>();
});

var host = builder.Build();
host.Run();
