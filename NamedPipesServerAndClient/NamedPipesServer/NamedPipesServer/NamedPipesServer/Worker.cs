using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO.Pipes;

namespace WinService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private Task? _noneBlockingTask;
    private readonly IConfiguration _config;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _config = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _noneBlockingTask = NoneBlockingOperation(cancellationToken).ContinueWith(async task =>
        {
            if (task.IsFaulted) return;

           await CallbackOperationAsync(cancellationToken);
        }, cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_noneBlockingTask != null)
        {
            await _noneBlockingTask;

            _noneBlockingTask?.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task NoneBlockingOperation(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
    }

    private async Task CallbackOperationAsync(CancellationToken cancellationToken)
    {
        string? pipeName = _config.GetValue<string>("PipeName");

        while (!cancellationToken.IsCancellationRequested)
        {
            using var server = new NamedPipeServerStream(string.IsNullOrEmpty(pipeName) ? "MyNamedPipe" : pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
            Console.WriteLine("Waiting for client connection...");

            var connectionCompletion = new TaskCompletionSource<bool>();

            server.WaitForConnectionAsync(cancellationToken).ContinueWith(async task =>
            {
                await HandleClientConnection(server, cancellationToken);
                connectionCompletion.SetResult(true);
            }, cancellationToken).ConfigureAwait(false).GetAwaiter();

            await connectionCompletion.Task;
        }
    }

    private async Task HandleClientConnection(NamedPipeServerStream pipe, CancellationToken cancellationToken)
    {
        try
        {
            using (var reader = new StreamReader(pipe))
            using (var writer = new StreamWriter(pipe))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Read the message from the client
                    var message = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(message))
                        break;

                    Console.WriteLine($"Received message: {message}");

                    // Process the message and send a response
                    var response = "Recieved";
                    await writer.WriteLineAsync(response);
                    await writer.FlushAsync();
                }
            }
        }
        catch (IOException)
        {
            //skip
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HandleClientConnection");
        }

    }
}