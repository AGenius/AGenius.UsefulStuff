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

# SSHelper (Screen Capture Helper))
A simple screen capture helper class to capture the screen and save it to a file

Allowing a hot key to be pressed to capture the screen and save it to a file or initiate a screen capture directly

To use the Screen Capture Helper, you can use the following code
```
SSHelper ss;
private void button1_Click(object sender, EventArgs e)
{
    ss = new SSHelper(); // Create a new instance of the SSHelper class
    ss.ScreenshotCaptured += Ss_ScreenshotCaptured; // Subscribe to the ScreenshotCaptured event
    ss.StartSSCapture();
}

private void Ss_ScreenshotCaptured(string screenshotPath)
{
    this.BringToFront();
    ss.Dispose();
            
    AGenius.UsefulStuff.Utils.LaunchURLorFile(screenshotPath);
}
```
or you could use the follow to wait for a hot key to be pressed to capture the screen
```
private void button1_Click(object sender, EventArgs e)
{
    ss = new SSHelper(Keys.F12); // Create a new instance of the SSHelper class and wait for the F12 key to be pressed
    ss.ScreenshotCaptured += Ss_ScreenshotCaptured; // Subscribe to the ScreenshotCaptured event
    ss.StartSSCapture();
}
```

