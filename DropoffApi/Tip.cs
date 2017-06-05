using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropoff
{
    public struct TipParameters
    {
        public string order_id;
        public string company_id;
        public double amount;
    }

    public class Tip
    {
        private Client client;

        internal Tip(Client client)
        {
            this.client = client;
        }

        public JObject Create(TipParameters parameters)
        {
            if (parameters.order_id == null)
            {
                throw new ArgumentException("order_id should not be null");
            }

            if (parameters.amount <= 0)
            {
                throw new ArgumentException("amount should pe a positive value");
            }

            Dictionary<string, string> query = new Dictionary<string, string>();

            if (parameters.company_id != null)
            {
                query.Add("company_id", parameters.company_id);
            }

            JObject result = client.DoPost("/order/" + parameters.order_id + "/tip/" + parameters.amount, "order", null, query);
            return result;
        }

        public JObject Get(TipParameters parameters)
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

            JObject result = client.DoGet("/order/" + parameters.order_id + "/tip", "order", query);
            return result;
        }

        public JObject Delete(TipParameters parameters)
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

            JObject result = client.DoDelete("/order/" + parameters.order_id + "/tip", "order", query);
            return result;
        }
    }
}
