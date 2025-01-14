namespace CLIChatAppClient;

class Program
{
    static void Main(string[] args)
    {
        string defaultServerAddress = "127.0.0.1";
        ChatClient chatClient = new ChatClient();
        chatClient.StartClient(defaultServerAddress, 4444);
        chatClient.CreateNewToken();
    }
}
