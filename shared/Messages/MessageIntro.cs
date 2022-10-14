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

        public Village Act(Client client) {
            Village? village = client.Villages.Find(x => x.Password == this.Password);
            Console.WriteLine("Village: " + village + " " + this.Password);
            if(village == null) {
                village = new Village(this.Password);
                client.Villages.Add(village);
            } 

            // send to all the other villagers how to connect to the new commer 
            village.Villagers.ForEach((v) => {
                IMessage message = new MessagePair(client.Pair);
                client.Send(message, v.Ip, v.port);
            });

            // add the villager to the village
            village.addVillager(new Villager(client.Pair.Address, client.Pair.Port));

            return village;
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