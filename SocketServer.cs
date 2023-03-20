using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServerH1
{
    internal class SocketServer
    {
        public SocketServer()
        {
            IPEndPoint endpoint = GetServerEndPoint();
            //Start server with endpoit perviously selected.

            Socket listener = new(
                endpoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            listener.Bind(endpoint);

            while(true)StartServer(listener);
        }

        private void StartServer(Socket listener)
        {
            
            listener.Listen(10);
            Console.WriteLine($"Server listening on: {listener.LocalEndPoint}");
            Socket handler = listener.Accept();
            Console.WriteLine($"Accepting connection from { handler.RemoteEndPoint}");

            string msg = null;
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRec = handler.Receive(buffer);
                msg += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                if (msg.IndexOf("<EOM>") > -1) break;
            }
            Console.WriteLine($"Message: {msg}");
        }

        public IPEndPoint GetServerEndPoint()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(strHostName);
            List<IPAddress> addrList = new();
            int counter = 0;
            foreach (var item in host.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetworkV6) continue;
                {
                    Console.WriteLine($"{counter++} {item.ToString()} {item.AddressFamily.ToString()}");
                    addrList.Add(item);
                }
            }
            int temp = 0;
            do { Console.WriteLine("Select server IP"); }
            while (!int.TryParse(Console.ReadLine(), out temp) || temp > addrList.Count || temp <0);
            
            IPAddress addr = addrList[temp];

            IPEndPoint localEndPoint = new IPEndPoint(addr, 11000);
            return localEndPoint;

        }
    }
}
