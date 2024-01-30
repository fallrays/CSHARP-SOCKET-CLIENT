using SocketClient.Network;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

SocketConnect socketConnect = new SocketConnect();
socketConnect.Connect();

string cmd = string.Empty;
while ((cmd = Console.ReadLine()) != "Q")
{
    if (cmd != null) 
    {
        socketConnect.SendMsg(cmd);
    }
}