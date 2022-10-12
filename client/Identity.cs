using IceColdMirror.Stun;

namespace AnimalCrossing {
    class Identity {
        public System.Net.IPAddress? Address { get; set; }
        public int Port { get; set; }

        public Identity() {
            this.update();
        }

        public void update() {
            var req = new StunMessage();
            var software = new StunAttribute(StunAttributeType.SOFTWARE, req);
            software.SetSoftware("Animal Crossing");
            req.attributes.Add(software);
            var clientUdp = new StunClientUdp();
            Task<StunMessage> res = clientUdp.SendRequest(req, "stun://stun.l.google.com:19302");
            res.Wait();
            StunMessage msg = res.Result;
            var indication = msg.attributes.First(a => a.Type == StunAttributeType.XOR_MAPPED_ADDRESS).GetXorMappedAddress();
            this.Address = indication.Address;
            this.Port = indication.Port;
        }
    }
}