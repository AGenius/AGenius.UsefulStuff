# AGenius.UsefulStuff
A Collection of useful utilities and Extensions

This contains a collection of stuff I constantly use in application development.  

I always find that I re-use so much useful and reused code so I have decided to put it together in one collection

I have added many usefull helpers and Utilites too

Full Comments and Documentation to follow

# TCPServer
Simple TCPServer classes for listening and transporting messages
from incomming client requests

# TCPClient
Simple TCPClient class for communicating to a server over TCP


# TCPSocketClient

A transactional based TCP Server using Socket (With or Without SSL) support

 >  Start the Server (With or Without SSL)
```
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var serverLogger = loggerFactory.CreateLogger<TcpServer>();

var certificate = new X509Certificate2("path/to/certificate.pfx", "password");
var server = new TCPSocketClient(5001, serverLogger, useSsl: true, certificate);
await server.StartAsync();

```
or without SSL
```
var server = new TcpServer(5001, serverLogger);
await server.StartAsync();
```

# TCPSocketClient
A transactional based TCP Client using Socket (With or Without SSL) support

> Start the Client
```
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TCPSocketClient>();
var client = new TCPSocketClient("127.0.0.1", 5001, logger, useSsl: true);

client.OnClientEvent += (eventType, message) => Console.WriteLine($"[{eventType}] {message}");

await client.ConnectAsync();
await client.SendMessageAsync("Hello, Secure Server!");
client.Disconnect();
```
or without SSL
```
var client = new TCPSocketClient("127.0.0.1", 5001, logger,);
```

