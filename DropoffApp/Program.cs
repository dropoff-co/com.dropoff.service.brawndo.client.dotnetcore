using System;
using Dropoff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DropoffApp
{
    class Program
    {
        protected ApiV1 brawndo;

        public Program()
        {
            brawndo = new ApiV1();
        }

        protected void Initialize()
        {
            string url = "https://sandbox-brawndo.dropoff.com/v1";
            string host = "sandbox-brawndo.dropoff.com";
            string private_key = "";
            string public_key = "";
            brawndo.Initialize(url, host, private_key, public_key);
        }

        protected JObject GetInfo()
        {
            JObject info = brawndo.Info();
            return info;
        }

        protected JObject AvailableProperties()
        {
            AvailablePropertiesParameters p = new AvailablePropertiesParameters();
            JObject properties = brawndo.order.AvailableProperties(p);
            return properties;
        }

        protected JObject GetOrderItems()
        {
            OrderItemsParameters p = new OrderItemsParameters();
            JObject items = brawndo.order.Items(p);
            return items;
        }

        protected JObject GetIndividualOrder(string order_id)
        {
            OrderGetParameters ogp = new OrderGetParameters();
            ogp.order_id = order_id;
            JObject order = brawndo.order.Get(ogp);
            return order;
        }

        protected JObject GetOrderSignature(string order_id)
        {
            OrderGetParameters ogp = new OrderGetParameters();
            ogp.order_id = order_id;
            JObject order = brawndo.order.GetSignature(ogp);
            return order;
        }

        protected JObject GetOrderPage(string last_key)
        {
            OrderGetParameters ogp = new OrderGetParameters();
            if (last_key != null)
            {
                ogp.last_key = last_key;
            }

            return brawndo.order.Get(ogp);
        }

        protected JObject CancelOrder(string order_id)
        {
            OrderCancelParameters cancelParameters = new OrderCancelParameters();
            cancelParameters.order_id = order_id;
            JObject cancelResponse = brawndo.order.Cancel(cancelParameters);
            return cancelResponse;
        }

        protected JObject GetEstimate(string origin, string destination, string utc_offset, Int32 ready_timestamp)
        {
            EstimateParameters estimateParams = new EstimateParameters();
            estimateParams.origin = origin;
            estimateParams.destination = destination;
            estimateParams.utc_offset = utc_offset;
            if (ready_timestamp > 0)
            {
                estimateParams.ready_timestamp = ready_timestamp;
            }
            JObject estimate = brawndo.order.Estimate(estimateParams);
            return estimate;
        }

        protected JObject CreateOrder(Int32 ready_date, JObject estimate, Int32[] properties)
        {
            OrderCreateParameters ocp = new OrderCreateParameters();
            ocp.origin = new OrderCreateAddress();
            ocp.origin.company_name = "Al's House";
            ocp.origin.first_name = "Algis";
            ocp.origin.last_name = "Woss";
            ocp.origin.address_line_1 = "2517 Thornton Road";
            //orderCreateParams.origin.address_line_2 = "";
            ocp.origin.city = "Austin";
            ocp.origin.state = "TX";
            ocp.origin.zip = "78704";
            ocp.origin.phone = "5124173167";
            ocp.origin.email = "awoss@dropoff.com";
            ocp.origin.lat = 30.242626;
            ocp.origin.lng = -97.772999;
            ocp.origin.remarks = "Origin Remarks";
            ocp.destination = new OrderCreateAddress();
            ocp.destination.company_name = "Dropoff";
            ocp.destination.first_name = "Algis";
            ocp.destination.last_name = "Woss";
            ocp.destination.address_line_1 = "800 Brazos Street";
            ocp.destination.address_line_2 = "Suite 250";
            ocp.destination.city = "Austin";
            ocp.destination.state = "TX";
            ocp.destination.zip = "78701";
            ocp.destination.phone = "555-555-5555";
            ocp.destination.email = "awoss+dropoff@dropoff.com";
            ocp.destination.lat = 30.270265;
            ocp.destination.lng = -97.741044;
            ocp.destination.remarks = "Destination Remarks";
            ocp.details = new OrderCreateDetails();
            ocp.details.ready_date = ready_date;
            ocp.details.type = "two_hr";
            ocp.details.quantity = 10;
            ocp.details.weight = 20;
            ocp.details.distance = (string)estimate["data"]["Distance"];
            ocp.details.eta = (string)estimate["data"]["ETA"];
            ocp.details.price = (string)estimate["data"]["two_hr"]["Price"];
            ocp.properties = properties;
            //orderCreateParams.details.reference_code = "";
            //orderCreateParams.details.reference_name = "";
            JObject cr = brawndo.order.Create(ocp);
            return cr;
        }

        protected JObject CreateTip(string order_id, double amount)
        {
            TipParameters tipParameters = new TipParameters();
            tipParameters.order_id = order_id;
            tipParameters.amount = amount;
            JObject tipResponse = brawndo.order.tip.Create(tipParameters);
            return tipResponse;
        }

        protected JObject GetTip(string order_id)
        {
            TipParameters tipParameters = new TipParameters();
            tipParameters.order_id = order_id;
            JObject tipResponse = brawndo.order.tip.Get(tipParameters);
            return tipResponse;
        }

        protected JObject DeleteTip(string order_id)
        {
            TipParameters tipParameters = new TipParameters();
            tipParameters.order_id = order_id;
            JObject tipResponse = brawndo.order.tip.Delete(tipParameters);
            return tipResponse;
        }
//
//        static void Main(string[] args)
//        {
//            Program p = new Program();
//
//            p.Initialize();
//            JObject i1 = p.GetInfo();
//            System.Diagnostics.Debug.WriteLine("Info: " + i1.ToString());
//            JObject props = p.AvailableProperties();
//            System.Diagnostics.Debug.WriteLine("Properties: " + props.ToString());
//            JObject signature = p.GetOrderSignature("gV1z-NVVE-O8w");
//            System.Diagnostics.Debug.WriteLine("Signature: " + signature.ToString());
//            JObject items = p.GetOrderItems();
//            System.Diagnostics.Debug.WriteLine("Items: " + items.ToString());
//
//            JObject e1 = p.GetEstimate(
//                "2517 Thornton Road, Austin, TX 78704", 
//                "800 Brazos Street, Austin, TX 78701",
//                DateTime.Now.ToString("zzz"),
//                0
//                );
//            System.Diagnostics.Debug.WriteLine("Estimate 1: " + e1.ToString());
//
//            DateTime tomorrow = DateTime.Now.AddDays(1);
//            DateTime tomorrowTenAM = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 10, 0, 0, 0);
//            if (tomorrowTenAM.Hour != 10)
//            {
//                tomorrowTenAM.AddHours(10 - tomorrowTenAM.Hour);
//            }
//            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
//            TimeSpan diff = tomorrowTenAM.ToUniversalTime() - origin;
//            JObject e2 =p.GetEstimate(
//                "2517 Thornton Road, Austin, TX 78704",
//                "800 Brazos Street, Austin, TX 78701",
//                tomorrowTenAM.ToString("zzz"),
//                (Int32) Math.Floor(diff.TotalSeconds)
//                );
//            System.Diagnostics.Debug.WriteLine("Estimate 2: " + e2.ToString());
//
//            JObject cr = p.CreateOrder((Int32)Math.Floor(diff.TotalSeconds), e2, new Int32[] { 9, 10 });
//            System.Diagnostics.Debug.WriteLine("Create Order Response: " + cr.ToString());
//
//            string created_order_id = (string)cr["data"]["order_id"];
//
//            JObject gor = p.GetIndividualOrder(created_order_id);
//            System.Diagnostics.Debug.WriteLine("Get Order Response: " + gor.ToString());
//
//            JObject tr = p.CreateTip(created_order_id, 4.44);
//            System.Diagnostics.Debug.WriteLine("Create Tip Response: " + tr.ToString());
//
//            tr = p.GetTip(created_order_id);
//            System.Diagnostics.Debug.WriteLine("Get Tip Response: " + tr.ToString());
//
//            tr = p.DeleteTip(created_order_id);
//            System.Diagnostics.Debug.WriteLine("Delete Tip Response: " + tr.ToString());
//
//            JObject cor = p.CancelOrder(created_order_id);
//            System.Diagnostics.Debug.WriteLine("Cancel Order Response: " + cor.ToString());
//        }
    }
}