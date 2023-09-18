namespace Socket_Server
{
    internal class Program
    {
        static Server Server = new();

        static void Main(string[] args)
        {
            Server.CreateHost();
            Console.WriteLine(Server.RetrieveClientTransfer());
            Server.TransferToClient();
            Server.CloseConnection();
        }
    }
}