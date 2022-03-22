using System;
using Newtonsoft.Json.Linq;

namespace Dropoff
{
    public class ApiV1
    {
        private Client client;
        public Order order;
        public Bulk bulk;

        public ApiV1()
        {

        }

        public void Initialize(string apiUrl, string host, string privateKey, string publicKey)
        {
            client = new Client(apiUrl, host, privateKey, publicKey);
            order = new Order(client);
            bulk = new Bulk(client);
        }

        public JObject Info()
        {
            JObject info = client.DoGet("/info", "info", null);
            return info;
        }
    }
}
