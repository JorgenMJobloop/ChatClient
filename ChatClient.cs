using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
/// <summary>
/// TCP Chat client class
/// </summary>
public class ChatClient
{
    private TcpClient tcpClient;
    private StreamWriter streamWriter;
    private StreamReader streamReader;

    public void StartClient(string serverAddress, int port)
    {
        try
        {
            tcpClient = new TcpClient(serverAddress, port);
            Console.WriteLine($"Connected to server!");
            var stream = tcpClient.GetStream();
            streamWriter = new StreamWriter(stream) { AutoFlush = true };
            streamReader = new StreamReader(stream);

            // open a new thread to recieve messages
            var readThread = new Thread(RecieveMessages);
            readThread.Start();
            // write messages to the server
            while (true)
            {
                string? message = Console.ReadLine();
                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }
                streamWriter.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to connect to the server! {e.Message}");
        }
    }

    private void RecieveMessages()
    {
        try
        {
            while (true)
            {
                var message = streamReader.ReadLine();
                if (message != null)
                {
                    Console.WriteLine($"Server: {message}");
                }
            }
        }
        catch
        {
            Console.WriteLine("Disconnected from server!");
        }
    }

}