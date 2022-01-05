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
        public string driver_actions;
        public bool? email_notifications;
        public bool? sms_notifications;
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
        public string  third_party_delivery_id;
        public string  air_waybill_number;
    }

    public class OrderCreateItem
    {
        public string sku = null;
        public Int32? quantity;
        public double? weight;
        public double? height;
        public double? width;
        public double? depth;
        public string unit = null;
        public Int32? container;
        public string description = null;
        public string price = null;
        public Int32? temperature;
        public string person_name = null;
    }

    public struct OrderCreateParameters
    {
        public string company_id;
        public OrderCreateAddress origin;
        public OrderCreateAddress destination;
        public OrderCreateDetails details;
        public Int32[] properties;
        public OrderCreateItem[] items;
    }

    public struct SimulateParameters
    {
        public string company_id;
        public string market;
        public string order_id;
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

    public struct DriverActionsMetaParameters
    {
        public string company_id;
    }

    public struct OrderItemsParameters
    {
        public string company_id;
    }

    public class Order
    {
        private Client client;
        public Tip tip;
        public Int32 TempNa = 0;
        public Int32 TempAmbient = 100;
        public Int32 TempRefrigerated = 200;
        public Int32 TempFrozen = 300;

        public Int32 ContainerNa = 0;
        public Int32 ContainerBag = 100;
        public Int32 ContainerBox = 200;
        public Int32 ContainerTray = 300;
        public Int32 ContainerPallet = 400;
        public Int32 ContainerBarrel = 500;
        public Int32 ContainerBasket = 600;
        public Int32 ContainerBucket = 700;
        public Int32 ContainerCarton = 800;
        public Int32 ContainerCase = 900;
        public Int32 ContainerCooler = 1000;
        public Int32 ContainerCrate = 1100;
        public Int32 ContainerTote = 1200;

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

        public JObject GetSignature(OrderGetParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            string path = "/order/signature";

            if (parameters.order_id != null)
            {
                path += "/" + parameters.order_id;
            } else
            {
                throw new ArgumentException("order_id is required");
            }

            JObject order = client.DoGet(path, "order", query);
            return order;
        }

        public JObject GetPickupSignature(OrderGetParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            string path = "/order/pickup_signature";

            if (parameters.order_id != null)
            {
                path += "/" + parameters.order_id;
            } else
            {
                throw new ArgumentException("order_id is required");
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

        public JObject DriverActionsMeta(DriverActionsMetaParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            JObject order = client.DoGet("/order/driver_actions_meta", "order", query);
            return order;
        }

        public JObject Items(OrderItemsParameters parameters)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }
            
            JObject items = client.DoGet("/order/items", "order", query);
            return items;
        }

        public JObject Simulate(SimulateParameters parameters)
        {
            if (parameters.market == null && parameters.order_id == null)
            {
                throw new ArgumentException("market or order_id should not be null");
            }

            string url = null;
            
            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            if (parameters.market != null)
            {
                url = "/order/simulate/" + parameters.market;
            } else if (parameters.order_id != null)
            {
                url = "/order/simulate/order/" + parameters.order_id;
            }
            
            JObject simulationResponse = client.DoGet(url, "order", query);
            return simulationResponse;
        }
    }
}
