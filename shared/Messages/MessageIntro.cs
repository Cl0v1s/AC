using System.Net;

namespace AnimalCrossing.Shared {
    public class MessageIntro : IMessage {
        public MessageTypes Type { get; set; }

        public string Password { get; set; }

        public MessageIntro(BinaryReader br) {
            this.Type = MessageTypes.Intro;

            this.Deserialize(br);
        }

        public MessageIntro(string password) {
            this.Type = MessageTypes.Intro;
            this.Password = password;
        }

        public void Act(IOther client)
        {
            client.Village ??= new Village(this.Password); 

            // send to all the other villagers how to connect to the new commer 
            client.Village.Villagers.ForEach((v) => {
                IMessage message = new MessagePair((client.Client.Client.RemoteEndPoint as IPEndPoint)!);
                v.Send(message);
            });

            client.Village.AddVillager(client);
        }

        public void Serialize(BinaryWriter bw) {
            bw.Write((int)this.Type);
            bw.Write(this.Password);
        }

        public void Deserialize(BinaryReader br) {
            // Type is already parsed
            this.Password = br.ReadString();
        } 
    }
}