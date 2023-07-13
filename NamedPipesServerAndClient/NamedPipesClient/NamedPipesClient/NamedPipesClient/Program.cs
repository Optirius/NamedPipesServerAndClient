using System.IO.Pipes;

namespace NamedPipeClientApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            string pipeName = "MyNamedPipe";
            int connectionTimeout = 5000; // Timeout period in milliseconds

            using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
            {
                Console.WriteLine("Connecting to the named pipe server...");
                var connectTask = client.ConnectAsync();

                // Wait for either the connection to succeed or the timeout period to elapse
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(connectionTimeout));

                if (completedTask == connectTask && client.IsConnected)
                {
                    Console.WriteLine("Connected to the named pipe server!");

                    while (true)
                    {
                        Console.Write("Enter a message to send to the server (or 'exit' to quit): ");
                        string message = Console.ReadLine();

                        if (string.IsNullOrEmpty(message))
                        {
                            Console.WriteLine($"Enter a valid message!");
                        }
                        else
                        {
                            if (message.ToLower() == "exit")
                                break;

                            // Send the message to the server
                            var writer = new StreamWriter(client);
                            await writer.WriteLineAsync(message);
                            await writer.FlushAsync();

                            // Read the response from the server
                            var reader = new StreamReader(client);
                            var response = await reader.ReadLineAsync();

                            Console.WriteLine($"Server response: {response}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to the named pipe server within the specified timeout period.");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
