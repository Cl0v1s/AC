using System.Net;

using System.Diagnostics;

namespace AnimalCrossing.Shared {

    public class Village {

        public string Password { get; }
        public List<IOther> Villagers { get; }

        public Village(string password) {
            this.Password = password;
            this.Villagers = new List<IOther>();
        }

        public void AddVillager(IOther villager)
        {
            IPEndPoint? villagerAddress = (villager.Client.Client.RemoteEndPoint as IPEndPoint);
            IOther? previous = this.Villagers.Find(x => (x.Client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() == villagerAddress?.Address.ToString());
            if(previous != null) {
                this.Villagers.Remove(previous);
            }
            this.Villagers.Add(villager);
            Console.WriteLine("Villager added: " + villagerAddress?.Address + ":" + villagerAddress?.Port);
        }


    }
}