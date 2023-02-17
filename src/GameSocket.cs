using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TicTacToe
{
    class GameSocket
    {
        Socket socket;
        Socket? clientSocket;
        string socketType;

        public GameSocket()
        {
            socketType = "host";
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[1]; // localhost
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 42069);

            socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen();
        }

        public GameSocket(IPAddress remoteHostIP)
        {
            socketType = "peer";
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteHostIP, 42069);

            socket = new Socket(remoteHostIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEndPoint);
            if (socket.RemoteEndPoint == null)
                throw new Exception("Error connecting to host");
            Console.WriteLine("Socket connected to -> {0} ", socket.RemoteEndPoint.ToString());
        }

        public void ListenForConnection()
        {
            if (socketType == "host")
            {
                if (socket.LocalEndPoint == null)
                    throw new Exception("Socket is not bound");


                Console.WriteLine("Listening for connections on {0}", socket.LocalEndPoint.ToString());
                clientSocket = socket.Accept();
            }
            else
            {
                throw new System.Exception("Cannot listen for connection on a peer socket");
            }
        }

        public void Send(string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);

            if (socketType == "host")
            {
                if (clientSocket == null)
                    throw new Exception("server must be connected to client to send message");
                clientSocket.Send(messageBytes);
            }
            else
            {
                socket.Send(messageBytes);
            }
        }

        public string Receive()
        {
            byte[] bytes = new byte[1024];
            int numByte;

            if (socketType == "host")
            {
                if (clientSocket == null)
                    throw new Exception("server must be connected to client to receive message");

                numByte = clientSocket.Receive(bytes);

            }
            else
            {
                numByte = socket.Receive(bytes);
            }

            string data = Encoding.ASCII.GetString(bytes, 0, numByte);
            return data;
        }
    }
}