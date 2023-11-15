namespace Socket_Server
{
    internal class Program
    {
        private static Host host;
        private static Connection? conenction;

        static void Main(string[] args)
        {
            host = new();

            host.Initialize();

            host.HostSocket.
            conenction = new(host); //Thread-able

            Console.ReadKey();
            host.Close();
        }
    }
}