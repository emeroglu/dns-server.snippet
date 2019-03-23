using DNS.Protocol.ResourceRecords;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DNS.Server
{
    class Program
    {
        public static bool redirect;

        static void Main(string[] args)
        {
            Server();    
        }

        private static void Raw()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Any, 53)); // specify IP address
            socket.ReceiveBufferSize = 2 * 1024; // 2 megabytes
            socket.ReceiveTimeout = 500; // half a second
            byte[] incoming = BitConverter.GetBytes(1);
            byte[] outgoing = BitConverter.GetBytes(1);
            socket.IOControl(IOControlCode.ReceiveAll, incoming, outgoing);

            byte[] buffer = null;

            while (buffer.Length < 20 * 1024)
            {
                socket.Receive(buffer);
            }

            if (true)
            {

            }

            Console.ReadKey();
        }

        private static void Server()
        {
            DnsServer server = new DnsServer("8.8.8.8");

            server.MasterFile.AddIPAddressResourceRecord("[DOMAIN]", "[IP]");

            server.Requested += Server_Requested;
            server.Responded += Server_Responded;

            Console.WriteLine("DNS Server is listening...");
            Console.WriteLine();

            server.Listen();
        }

        private static void Server_Requested(Protocol.IRequest request)
        {
            
        }

        private static void Server_Responded(Protocol.IRequest request, Protocol.IResponse response)
        {
            string domain = request.Questions[0].Name.ToString();

            List<IResourceRecord> listRecords = response.AnswerRecords.Where(r => r.Type == Protocol.RecordType.A).ToList();

            Console.WriteLine(response);

            string ips = "";

            foreach (IResourceRecord record in listRecords)
            {
                ips += ((IPAddressResourceRecord)record).IPAddress.ToString() + ", ";
            }

            Console.WriteLine(domain + ": " + ips);
            Console.WriteLine();
        }
    }
}
