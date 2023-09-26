using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Socket_Server
{
    public class Host
    {
        #region Variables & Properties
        public Socket HostSocket { get; private set; }
        public IPEndPoint EndPoint { get; private set; }

        public int Port { get; private set; }
        public int MaxClients { get; private set; }
        #endregion

        public Host(int port = 1111, int maxClients = 1)
        {
            Port = port;
            MaxClients = maxClients;
        }

        public void Initialize()
        {
            IPHostEntry IPEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress IPAddr = IPEntry.AddressList[0];
            IPEndPoint EndPoint = new(IPAddr, Port);
            this.EndPoint = EndPoint;

            HostSocket = new(IPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                HostSocket.Bind(EndPoint);
                HostSocket.Listen(MaxClients);

                Console.WriteLine($"Created Host on EndPoint {EndPoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Close()
        {
            try
            {
                HostSocket.Shutdown(SocketShutdown.Both);
                HostSocket.Close();

                Console.WriteLine($"Server is closed"); //CW Feedback
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Connection // Add and use model-controller layer to save all retrieved data and verify
    {
        #region Variables & Properties
        private Host host;
        private Socket clientSocket { get; set; }

        private byte[] bufferData { get; set; }
        private byte[] submitData { get; set; }
        //private int bufferSize { get; set; } = 1024;
        private string bufferDecoded { get; set; }
        private bool isPendingConnection { get; set; } = true;
        #endregion

        public Connection(Host host)
        {
            this.host = host;
            Establish(); 
        }

        public void Establish()
        {
            //bufferData = new byte[bufferSize];

            Console.WriteLine("Connection is Pending ...");

            do
            {
                try
                {
                    clientSocket = host.HostSocket.Accept();

                    if (clientSocket.Connected)
                    {
                        Console.WriteLine("Client is connection");
                        isPendingConnection = false;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            while (isPendingConnection);

            Await();
        }

        private void Await()
        {
            try
            {
                do
                {
                    if (clientSocket.Available > 0) // If Available data from the client is ready to be read
                    {
                        int packedLenght = clientSocket.Receive(bufferData);
                        bufferDecoded = Encoding.ASCII.GetString(bufferData, 0, packedLenght);

                        Submit(bufferDecoded);
                    }
                }
                while (clientSocket.Connected);
            }
            catch (Exception e)
            {
                _ = e;
                Console.WriteLine(e);
            }
            finally
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    Console.WriteLine("Connection to client is now closed");
                }
            }
        }

        private void Submit(string message)
        {
            // Add logic for which answer to send/action to execute
            submitData = Encoding.ASCII.GetBytes("Answer");
            clientSocket.Send(submitData);

            if (message.Contains("<FIN>"))
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
