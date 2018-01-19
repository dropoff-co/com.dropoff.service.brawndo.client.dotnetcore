using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropoff
{
    public struct OrderCreateAddress
    {
        public string address_line_1;
        public string address_line_2;
        public string company_name;
        public string first_name;
        public string last_name;
        public string phone;
        public string email;
        public string city;
        public string state;
        public string zip;
        public double lat;
        public double lng;
        public string remarks;
    }

    public struct OrderCreateDetails
    {
        public Int32   quantity;
        public Int32   weight;
        public string  eta;
        public string  distance;
        public string  price;
        public Int32   ready_date;
        public string  type;
        public string  reference_name;
        public string  reference_code;
    }

    public struct OrderCreateParameters
    {
        public string company_id;
        public OrderCreateAddress origin;
        public OrderCreateAddress destination;
        public OrderCreateDetails details;
        public Int32[] properties;
    }

    public struct EstimateParameters
    {
        public string origin;
        public string destination;
        public string utc_offset;
        public Int32 ready_timestamp;
        public string company_id;
    }

    public struct OrderGetParameters
    {
        public string order_id;
        public string company_id;
        public string last_key;
    }

    public struct OrderCancelParameters
    {
        public string order_id;
        public string company_id;
        public string last_key;
    }

    public struct AvailablePropertiesParameters
    {
        public string company_id;
    }

    public class Order
    {
        private Client client;
        public Tip tip;

        internal Order(Client client)
        {
            this.client = client;
            tip = new Tip(client);
        }

        public JObject Estimate(EstimateParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.origin == null)
            {
                throw new ArgumentException("origin should not be null");
            } else
            {
                query.Add("origin", parameters.origin);
            }

            if (parameters.destination == null)
            {
                throw new ArgumentException("origin should not be null");
            }
            else
            {
                query.Add("destination", parameters.destination);
            }

            if (parameters.utc_offset == null)
            {
                throw new ArgumentException("utc_offset should not be null");
            }
            else
            {
                query.Add("utc_offset", parameters.utc_offset);
            }

            if (parameters.ready_timestamp > 0)
            {
                query.Add("ready_timestamp", parameters.ready_timestamp.ToString());
            }

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            JObject estimate = client.DoGet("/estimate", "estimate", query);
            return estimate;
        }

        public JObject Get(OrderGetParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            if (parameters.last_key != null)
            {
                query.Add("last_key", parameters.last_key);
            }

            string path = "/order";

            if (parameters.order_id != null)
            {
                path += "/" + parameters.order_id;
            }

            JObject order = client.DoGet(path, "order", query);
            return order;
        }

        public JObject Cancel(OrderCancelParameters parameters)
        {
            if (parameters.order_id == null)
            {
                throw new ArgumentException("order_id should not be null");
            }

            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            JObject result = client.DoPost("/order/" + parameters.order_id + "/cancel", "order", null, query);
            return result;
        }

        public JObject Create(OrderCreateParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            string payload = JsonConvert.SerializeObject(parameters, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            JObject result = client.DoPost("/order", "order", payload, query);
            return result;
        }

        public JObject AvailableProperties(AvailablePropertiesParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            JObject order = client.DoGet("/order/properties", "order", query);
            return order;
        }

        public JObject Simulate(string market)
        {
            if (market == null)
            {
                throw new ArgumentException("market should not be null");
            }

            JObject order = client.DoGet("/order/simulate/" + market, "order", null);
            return order;
        }
    }
}
