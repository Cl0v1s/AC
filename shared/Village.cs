using System.Net;

using System.Diagnostics;

namespace AnimalCrossing.Shared {

    public class Villager {
        public IPAddress Ip { get; set; }

        public int port { get; set; }

        public Villager(IPAddress ip, int port) {
            this.Ip = ip;
            this.port = port;
        }
    }

    public class Village {

        public string Password { get; }
        public List<Villager> Villagers { get; }

        public Village(string password) {
            this.Password = password;
            this.Villagers = new List<Villager>();
        }

        public void addVillager(Villager villager) {
            Villager? previous = this.Villagers.Find(x => x.Ip == villager.Ip);
            if(previous != null) {
                this.Villagers.Remove(previous);
            }
            this.Villagers.Add(villager);
            Console.WriteLine("Villager added: " + villager.Ip.ToString() + ":" + villager.port);
        }


    }
}