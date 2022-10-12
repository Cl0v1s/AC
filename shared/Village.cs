using System.Net;

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
        private List<Villager> villagers;

        public Village(string password) {
            this.Password = password;
            this.villagers = new List<Villager>();
        }

        public void addVillager(Villager villager) {
            Villager? previous = this.villagers.Find(x => x.Ip == villager.Ip);
            if(previous != null) {
                this.villagers.Remove(previous);
            }
            this.villagers.Add(villager);
        }


    }
}