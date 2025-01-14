using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// TCP Chat client class
/// </summary>
public class ChatClient
{
    private TcpClient? tcpClient;
    private StreamWriter? streamWriter;
    private StreamReader? streamReader;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    /// <summary>
    /// Start a new Client-connection to the P2P server
    /// </summary>
    /// <param name="serverAddress">the IP Address of the server</param>
    /// <param name="port">The port the server is open on</param>
    public void StartClient(string serverAddress, int port)
    {
        try
        {
            // initalize the connection
            tcpClient = new TcpClient(serverAddress, port);
            Console.WriteLine($"Connected to server!");
            // open up the stream
            var stream = tcpClient.GetStream();
            streamWriter = new StreamWriter(stream) { AutoFlush = true };
            streamReader = new StreamReader(stream);


            // send the client-token to the server recipient
            string? clientUUID = Guid.NewGuid().ToString();
            string? hashedToken = GenerateNewSHA256Hash(clientUUID);
            SendToken(hashedToken); // send the token to the server

            // open a new thread to recieve messages
            var readThread = new Thread(RecieveMessages);
            readThread.Start();
            // write messages to the server
            Console.WriteLine("Type 'exit' to disconnect.");

            while (true)
            {
                string? message = Console.ReadLine();
                if (message?.ToLower() == "exit")
                {
                    StopClient();
                    break;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    streamWriter.WriteLine(message);
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Failed to connect to the server! {e.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured {ex.Message}");
        }
    }
    /// <summary>
    /// Send a token to the server from the client
    /// </summary>
    /// <param name="token">the token to send</param>
    private void SendToken(string token)
    {
        streamWriter?.WriteLine($"TOKEN: {token}");
        Console.WriteLine($"Sent token: {token}");
    }
    /// <summary>
    /// Recieve messages from the server
    /// </summary>
    private void RecieveMessages()
    {
        var token = cancellationTokenSource.Token;
        try
        {
            while (!token.IsCancellationRequested)
            {
                var message = streamReader?.ReadLine();
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
    /// <summary>
    /// Stop the client connection
    /// </summary>
    public void StopClient()
    {
        cancellationTokenSource.Cancel();
        streamWriter?.Close();
        streamReader?.Close();
        tcpClient?.Close();
        Console.WriteLine("Disconnected from the server!");
    }
    /// <summary>
    /// Generate a new SHA256-Hash
    /// </summary>
    /// <param name="rawData">the input data to hash</param>
    /// <returns>the hashed output of rawData</returns>
    /// <exception cref="ArgumentException">Raise an exception if the arguments are passed incorrectly</exception>
    private string GenerateNewSHA256Hash(string rawData)
    {
        if (string.IsNullOrEmpty(rawData))
        {
            throw new ArgumentException("The input cannot be empty or null");
        }
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(hash);
        }
    }
    /// <summary>
    /// Generate a new user token and save it to a file called user_token.txt locally
    /// </summary>
    private void GenerateUserToken()
    {
        const string filePath = "user_token.txt";

        if (!File.Exists("user_token.txt"))
        {
            using (var FileStream = File.Create(filePath)) { }
        }
        //nint handle = tcpClient.GetHashCode();
        string clientUUID = Guid.NewGuid().ToString();
        string? hashUUID = GenerateNewSHA256Hash(clientUUID);
        Console.WriteLine($"clientUUID");
        Console.WriteLine($"hashUUID");
        File.WriteAllText(filePath, "Client token: " + hashUUID);
    }
    /// <summary>
    /// Used for testing the hashing method(s)
    /// </summary>
    public void CreateNewToken()
    {
        GenerateUserToken();
    }
}