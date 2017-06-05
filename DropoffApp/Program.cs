using System;
using Dropoff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DropoffApp
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Debug.WriteLine("Hello World!");
            Dropoff.ApiV1 api = new Dropoff.ApiV1();
            string url = "http://192.168.1.95:9094/v1";
            string host = "192.168.1.95:9094";
            string private_key = "7f8fee62743d7bb5bf2e79a0438516a18f4a4a4df4d0cfffda26a3b906817482";
            string public_key = "user::91e9b320b0b5d71098d2f6a8919d0b3d5415db4b80d4b553f46580a60119afc8";

            api.Initialize(url, host, private_key, public_key);
            JObject info = api.Info();
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("Info: " + info.ToString());

            OrderGetParameters orderGetParams = new OrderGetParameters();

            JObject page = api.order.Get(orderGetParams);
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("Order Page: " + page.ToString());

            string page1LastKey = (string)page["last_key"];

            if (page["last_key"] != null)
            {
                orderGetParams.last_key = (string) page["last_key"];
            }

            page = api.order.Get(orderGetParams);
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("Order Page 2: " + page.ToString());
            string page2LastKey = (string)page["last_key"];

            System.Diagnostics.Debug.WriteLine("page1LastKey: " + page1LastKey);
            System.Diagnostics.Debug.WriteLine("page2LastKey: " + page2LastKey);
            System.Diagnostics.Debug.WriteLine("last keys are equal? " + (page1LastKey == page2LastKey));
            string order_id = (string)page["data"][0]["details"]["order_id"];
            orderGetParams = new OrderGetParameters();
            orderGetParams.order_id = order_id;
            JObject anOrder = api.order.Get(orderGetParams);
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("An order: " + anOrder.ToString());

            EstimateParameters estimateParams = new EstimateParameters();
            estimateParams.origin = "2517 Thornton Road, Austin, TX 78704";
            estimateParams.destination = "800 Brazos Street, Austin, TX 78701";
            estimateParams.utc_offset = DateTime.Now.ToString("zzz");
            JObject estimate = api.order.Estimate(estimateParams);
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("Estimate: " + estimate.ToString());
            DateTime tomorrow = DateTime.Now.AddDays(1);
            DateTime tomorrowTenAM = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 10, 0, 0, 0);
            if (tomorrowTenAM.Hour != 10)
            {
                tomorrowTenAM.AddHours(10 - tomorrowTenAM.Hour);
            }
            estimateParams.utc_offset = tomorrowTenAM.ToString("zzz");

            System.Diagnostics.Debug.WriteLine(tomorrowTenAM.ToString());

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = tomorrowTenAM.ToUniversalTime() - origin;
            estimateParams.ready_timestamp = (Int32)Math.Floor(diff.TotalSeconds);
            estimate = api.order.Estimate(estimateParams);
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("+++++++++++++++++++++++++++++++++++++");
            System.Diagnostics.Debug.WriteLine("Estimate 2: " + estimate.ToString());

            OrderCreateParameters orderCreateParams = new OrderCreateParameters();
            orderCreateParams.origin = new OrderCreateAddress();
            orderCreateParams.origin.company_name = "Al's House";
            orderCreateParams.origin.first_name = "Algis";
            orderCreateParams.origin.last_name = "Woss";
            orderCreateParams.origin.address_line_1 = "2517 Thornton Road";
            //orderCreateParams.origin.address_line_2 = "";
            orderCreateParams.origin.city = "Austin";
            orderCreateParams.origin.state = "TX";
            orderCreateParams.origin.zip = "78704";
            orderCreateParams.origin.phone = "5124173167";
            orderCreateParams.origin.email = "awoss@dropoff.com";
            orderCreateParams.origin.lat = (double)estimate["data"]["coordinates"]["pickup"]["lat"];
            orderCreateParams.origin.lng = (double)estimate["data"]["coordinates"]["pickup"]["lng"];
            orderCreateParams.origin.remarks = "Origin Remarks";
            orderCreateParams.destination = new OrderCreateAddress();
            orderCreateParams.destination.company_name = "Dropoff";
            orderCreateParams.destination.first_name = "Algis";
            orderCreateParams.destination.last_name = "Woss";
            orderCreateParams.destination.address_line_1 = "800 Brazos Street";
            orderCreateParams.destination.address_line_2 = "Suite 250";
            orderCreateParams.destination.city = "Austin";
            orderCreateParams.destination.state = "TX";
            orderCreateParams.destination.zip = "78701";
            orderCreateParams.destination.phone = "555-555-5555";
            orderCreateParams.destination.email = "awoss+dropoff@dropoff.com";
            orderCreateParams.destination.lat = (double) estimate["data"]["coordinates"]["destination"]["lat"];
            orderCreateParams.destination.lng = (double) estimate["data"]["coordinates"]["destination"]["lng"];
            orderCreateParams.destination.remarks = "Destination Remarks";
            orderCreateParams.details = new OrderCreateDetails();
            orderCreateParams.details.ready_date = (Int32)Math.Floor(diff.TotalSeconds);
            orderCreateParams.details.type = "two_hr";
            orderCreateParams.details.quantity = 10;
            orderCreateParams.details.weight = 20;
            orderCreateParams.details.distance = (string) estimate["data"]["Distance"];
            orderCreateParams.details.eta = (string) estimate["data"]["ETA"];
            orderCreateParams.details.price = (string)estimate["data"]["two_hr"]["Price"];
            //orderCreateParams.details.reference_code = "";
            //orderCreateParams.details.reference_name = "";
            string json = JsonConvert.SerializeObject(orderCreateParams);
            System.Diagnostics.Debug.WriteLine(json);
            JObject createResponse = api.order.Create(orderCreateParams);
            System.Diagnostics.Debug.WriteLine(createResponse);
            string created_order_id = (string)createResponse["data"]["order_id"];
            System.Diagnostics.Debug.WriteLine(createResponse);
            TipParameters tipParameters = new TipParameters();
            tipParameters.order_id = created_order_id;
            tipParameters.amount = 4.44;
            JObject tipResponse = api.order.tip.Create(tipParameters);
            System.Diagnostics.Debug.WriteLine(tipResponse);
            tipResponse = api.order.tip.Get(tipParameters);
            System.Diagnostics.Debug.WriteLine(tipResponse);
            tipResponse = api.order.tip.Delete(tipParameters);
            System.Diagnostics.Debug.WriteLine(tipResponse);
            OrderCancelParameters cancelParameters = new OrderCancelParameters();
            cancelParameters.order_id = created_order_id;
            JObject cancelResponse = api.order.Cancel(cancelParameters);
            System.Diagnostics.Debug.WriteLine(cancelResponse);
        }
    }
}