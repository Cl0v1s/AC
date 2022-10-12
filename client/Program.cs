using System;
using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing
{
    class Program
    {
        public static Identity identity = new Identity();

        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("localhost", int.Parse(args[0]));
            Console.WriteLine("Connected");
            NetworkStream s = client.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Message from Client$");
            s.Write(outStream, 0, outStream.Length);
        }
    }
}