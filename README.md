# NamedPipesServerAndClient

This repository contains two sources:

## 1) NamedPipesServer

This is a console/windows service application that can be used to host a Named Pipe Server. The Named Pipe Server listens for incoming connections and handles communication with connected clients over named pipes.

### Usage

1. Clone or download this repository.
2. Open the `NamedPipesServer` solution in Visual Studio or your preferred development environment.
3. Build the solution to compile the `NamedPipesServer` application.
4. Run the application, and it will start hosting the Named Pipe Server.
5. The server will listen for incoming connections and handle communication with connected clients.

### Configuration

You can modify the server's behavior and settings by adjusting the configuration options in the `appsettings.json` file.

### Logging

The server application uses Serilog along with Graylog for enhanced logging capabilities. The logging configuration can be customized in the `appsettings.json` file. By default Graylog is disabled.

## 2) NamedPipesClient

This is a console application that can be used to connect to the Named Pipe Server and send messages. The client application can establish a connection with the Named Pipe Server and send messages that the server can receive and process.

### Usage

1. Open the `NamedPipesClient` solution in Visual Studio or your preferred development environment.
2. Build the solution to compile the `NamedPipesClient` application.
3. Run the client application.
4. The client will connect to the Named Pipe Server and send messages to it.
5. The server will receive and process the messages sent by the client.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, feel free to submit a pull request or create an issue.

## License

This repository is licensed under the MIT License. See the [LICENSE](/LICENSE) file for details.
