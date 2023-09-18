namespace Socket_Server
{
    internal class Program
    {
        static Server Server = new();

        static void Main(string[] args)
        {
            Server.CreateHost();

            Console.ReadKey();
            Server.CloseConnection();
        }
    }
}