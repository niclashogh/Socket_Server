using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Socket_Server
{
    public class Server_Old
    {
        #region Variables
        private Socket clientSocket;
        private Socket serverSocket;

        private byte[] bufferArray;
        private byte[] dataSubmit;
        private string bufferDecoded;

        private bool isPendingConnection = true;
        #endregion

        public void CreateHost(int clientMax = 1, int port = 1111) // = Defualt
        {
            IPHostEntry IPEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress IPAddr = IPEntry.AddressList[0];
            IPEndPoint EndPoint = new(IPAddr, port);

            serverSocket = new(IPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Bind(EndPoint);
                serverSocket.Listen(clientMax);

                Console.WriteLine($"Created Host on EndPoint {EndPoint}");

                SetupClientConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetupClientConnection(int bufferSize = 1024) // = Defualt
        {
            try
            {
                do
                {
                    clientSocket = serverSocket.Accept();
                    bufferArray = new byte[bufferSize];

                    if (clientSocket.Connected)
                        RetrieveClientTransfer();
                }
                while (isPendingConnection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RetrieveClientTransfer()
        {
            try
            {
                bool isEOF = false;
                do
                {
                    int packedLength = clientSocket.Receive(bufferArray);
                    bufferDecoded += Encoding.ASCII.GetString(bufferArray, 0, packedLength);

                    Console.WriteLine($"Message from Client : {bufferDecoded}"); //CW Feedback

                    if (bufferDecoded.IndexOf("<EOF>") > -1)
                    {
                        isEOF = true;
                        isPendingConnection = false;

                        TransferToClient();
                    }
                }
                while (!isEOF);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void TransferToClient(string message = "Server Respons<EOF>") // = Defualt
        {
            try
            {
                dataSubmit = Encoding.ASCII.GetBytes(message);
                clientSocket.Send(dataSubmit);

                Console.WriteLine($"Message to Client : {message}"); //CW Feedback
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //CloseConnection();
            }
        }

        public void CloseConnection()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                Console.WriteLine($"Closed Conenction to Client"); //CW Feedback
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class TestServer
    {
        public void Run()
        {
            IPHostEntry IPEntry = Dns.GetHostEntry(Dns.GetHostName()); //Retrives a list of HostEntries
            IPAddress IPAddr = IPEntry.AddressList[0]; //Select the first in the list (usually the active)
            IPEndPoint EndPoint = new(IPAddr, 1111); //Set new endpoint obj (IPAddr, #Port)

            // New Socket to listen on
            Socket Listener = new(IPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Listener.Bind(EndPoint); //Binds the EndPoint to the Socket
                Listener.Listen(1); //Backlog List Size (Enables multiple user to submit data to the Socket, by using lists and backloggin)

                bool isPending = true;
                do
                {
                    Console.WriteLine($"Server Endpoint : {EndPoint}"); //CW Feedback
                    Console.WriteLine("Connection Pending ..."); //CW Feedback

                    Socket clientSocket = Listener.Accept(); //Set the newly astablished connection to a socket obj.
                    Console.WriteLine($"Connection to a Client is established");
                    byte[] bufferArray = new byte[1024]; //WinSize ??
                    string bufferDecoded = null; //String buffer for decoded data

                    bool isEOF = false;
                    do
                    {
                        int packetLenght = clientSocket.Receive(bufferArray);
                        bufferDecoded += Encoding.ASCII.GetString(bufferArray, 0, packetLenght);

                        if (bufferDecoded.IndexOf("<EOF>") > -1) //EOF : End of File
                            isEOF = true;

                    }
                    while (!isEOF);

                    Console.WriteLine($"Message from Client : {bufferDecoded}"); //CW Feedback
                    byte[] dataTransfer = Encoding.ASCII.GetBytes("Test 1 <Server>");
                    clientSocket.Send(dataTransfer); //Send via the Socket connected to the Client

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                while (isPending);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
