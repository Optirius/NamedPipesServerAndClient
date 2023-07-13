using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog.Exceptions;
using WinService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        IConfiguration configuration = hostContext.Configuration;

        //Serilog Configuration
        var loggerConfiguration = new LoggerConfiguration()
                                        .Enrich.WithExceptionDetails()
                                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);

        if (configuration.GetValue<bool>("Serilog:File:Enabled"))
        {
            loggerConfiguration.WriteTo.File(
                path: AppDomain.CurrentDomain.BaseDirectory + configuration.GetValue<string>("Serilog:File:Path"),
                outputTemplate: "{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Properties}",
                encoding: Encoding.UTF8);
        }
        if (configuration.GetValue<bool>("Serilog:Graylog:Enabled"))
        {
            loggerConfiguration.WriteTo.Graylog(new GraylogSinkOptions
            {
                HostnameOrAddress = configuration.GetValue<string>("Serilog:Graylog:HostnameOrAddress"),
                Port = configuration.GetValue<int>("Serilog:Graylog:Port"),
                TransportType = Serilog.Sinks.Graylog.Core.Transport.TransportType.Udp,
                Facility = $"{configuration.GetValue<string>("Serilog:Graylog:Facility")}-{configuration.GetValue<string>("PipeName")}",
                MinimumLogEventLevel = LogEventLevel.Information,
            });
        }
        Log.Logger = loggerConfiguration.CreateLogger();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
