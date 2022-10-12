using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        public static Identity identity = new Identity();

        static void Main(string[] args)
        {

            if(Program.identity.Address == null) {
                Console.WriteLine("Unable to get identity");
                return;
            }

            TcpClient client = new TcpClient();
            client.Connect("localhost", int.Parse(args[0]));
            Console.WriteLine("Connected");
            NetworkStream s = client.GetStream();

            MemoryStream memory = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memory);

            MessageIntro msgIntro = new MessageIntro("bonjour", Program.identity.Address, Program.identity.Port);
            msgIntro.Serialize(writer);

            s.Write(memory.ToArray());

            s.Close();
            client.Close();
        }
    }
}