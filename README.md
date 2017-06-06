

# com.dropoff.service.brawndo.client.dotnetcore

This is the 3rd party dropoff .NET client for creating and viewing orders.

* **For Javascript documentation go [HERE](https://github.com/dropoff-co/com.dropoff.service.brawndo.client/blob/master/README.md "Javascript")**
* **For PHP documentation go [HERE](https://github.com/dropoff-co/com.dropoff.service.brawndo.client/blob/master/php_README.md "PHP")**
* **For GO documentation go [HERE](https://github.com/dropoff-co/com.dropoff.service.brawndo.client/blob/master/go_README.md "GO")**
* **For Ruby documentation go [HERE](https://github.com/dropoff-co/com.dropoff.service.brawndo.client/blob/master/ruby_README.md "Ruby")**

# Table of Contents
  + [Client Info](#client)
    - [Configuration](#configuration)
    - [Getting Your Account Info](#client_info)
    - [Enterprise Managed Clients](#managed_clients)
    - [Getting Pricing Estimates](#estimates)
    - [Placing an Order](#placing)
    - [Cancelling an Order](#cancel)
    - [Getting a Specific Order](#specific)
    - [Getting a Page of Order](#page)
  + [Tips](#tips)  
    - [Creating](#tip_create)
    - [Deleting](#tip_delete)
    - [Reading](#tip_read)
  + [Webhook Info](#webhook)
    - [Webhook Backoff Algorithm](#backoff)
    - [Webhook Events](#events)
    - [Managed Client Events](#managed_client_events)
  + [Order Simulation](#simulation)

## Using the client <a id="client"></a>

Instantiate an instance of ApiV1 in order to start making calls to brawndo.

    using System;
    using Dropoff;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
    namespace DropoffExample
    {
        class Program
        {
            static void Main(string[] args)
            {
                ApiV1 brawndo = new ApiV1();
            }
        }
    }


### Configuration <a id="configuration"></a>

You will then have to configure the brawndo instance with the configure function.

    string api_url = "https://sandbox-brawndo.dropoff.com/v1";
    string host = "sandbox-brawndo.dropoff.com";
    string private_key = "fcb60b8680d7b5c67921a852b39067a19d85318ce8abf4c512";
    string public_key = "ced2eaf24f1eaf832c1ea92b41386b4a5982cfcdb69b7c7818";

    brawndo.Initialize(api_url, host, private_key, public_key);
            
* **api_url** - the url of the brawndo api.  This field is required.
* **host** - the api host.  This field is required.
* **public_key** - the public key of the user that will be using the client.  This field is required.
* **private_key** - the private key of the user that will be using the client.


### Getting Your Client Information <a id="client_info"></a>

If you want to know your client id and name you can access this information via the info call.

If you are an enterprise client user, then this call will return all of the accounts that you are allowed to manage with your current account.

    JObject info = brawndo.Info();
    
Note that it returns a JObject instance.  The .NET brawndo api has a dependency on Newtonsoft Json.NET framework.  You can get more information on that [here](http://www.newtonsoft.com/json).
    
A response will look like this:

    {
      success: true
      timestamp: "2017-01-25T16:51:36Z",
      data: {
        client: {
          company_name: "EnterpriseCo Global",
          id: "1111111111110"
        },
        user: {
          first_name: "Algis",
          last_name: "Woss",
          id: "2222222222222"
        },
        managed_clients: {
          level: 0,
          company_name: "EnterpriseCo Global",
          id: "1111111111110"
          children : [
            {
              level: 1,
              company_name: "EnterpriseCo Europe",
              id: "1111111111112"
              children : [
                {
                  level: 2,
                  company_name: "EnterpriseCo Paris",
                  id: "1111111111111"
                  children : []
                },
                {
                  level: 2,
                  company_name: "EnterpriseCo London",
                  id: "1111111111113"
                  children : []
                },
                {
                  level: 2,
                  company_name: "EnterpriseCo Milan",
                  id: "1111111111114"
                  children : []
                }
              ]
            },
            {
              level: 1,
              company_name: "EnterpriseCo NA",
              id: "1111111111115"
              children : [
                {
                  level: 2,
                  company_name: "EnterpriseCo Chicago",
                  id: "1111111111116"
                  children : []
                },
                {
                  level: 2,
                  company_name: "EnterpriseCo New York",
                  id: "1111111111117"
                  children : []
                },
                {
                  level: 2,
                  company_name: "EnterpriseCo Los Angeles",
                  id: "1111111111118"
                  children : []
                }
              ]
            }
          ]
        }
      }
    }
    
The main sections in data are user, client, and managed_clients.  

The user info shows basic information about the Dropoff user that the used keys represent.

The client info shows basic information about the Dropoff Client that the user belongs to who's keys are being used.

The managed_clients info shows a hierarchical structure of all clients that can be managed by the user who's keys are being used.

### Enterprise Managed Clients  <a id="managed_clients"></a>

In the above info example you see that keys for a user in an enterprise client are being used.  It has clients that can be managed as it's descendants.

The hierarchy looks something like this:


        EnterpriseCo Global (1111111111110)
        ├─ EnterpriseCo Europe (1111111111112)
        │  ├─ EnterpriseCo Paris (1111111111111)
        │  ├─ EnterpriseCo London (1111111111113)
        │  └─ EnterpriseCo Milan (1111111111114)
        └─ EnterpriseCo NA (1111111111115)
           ├─ EnterpriseCo Chicago (1111111111116)
           ├─ EnterpriseCo New York (1111111111117)
           └─ EnterpriseCo Los Angeles (1111111111118)


Let's say I was using keys for a user in **EnterpriseCo Europe**, then the returned hierarchy would be:

        EnterpriseCo Europe (1111111111112)
        ├─ EnterpriseCo Paris (1111111111111)
        ├─ EnterpriseCo London (1111111111113)
        └─ EnterpriseCo Milan (1111111111114)
        
Note that You can no longer see the **EnterpriseCo Global** ancestor and anything descending and including **EnterpriseCo NA**.


So what does it mean to manage an enterprise client?  This means that you can:

- Get estimates for that client.
- Place an order for that client.
- Cancel an order for that client.
- View existing orders placed for that client.
- Create, update, and delete tips for orders placed for that client.

All you have to do is specify the id of the client that you want to act on.  So if wanted to place orders for **EnterpriseCo Paris** I would make sure to include that clients id: "1111111111111".

The following api documentation will show how to do this.


### Getting Pricing Estimates <a id="estimates"></a>

Before you place an order you will first want to estimate the distance, eta, and cost for the delivery.  The client provides an **Estimate** function for this operation.

    EstimateParameters estimateParams = new EstimateParameters();
    estimateParams.origin = "117 San Jacinto Blvd, Austin, TX 78701";
    estimateParams.destination = "901 S MoPac Expy, Austin, TX 78746";
    estimateParams.utc_offset = DateTime.Now.ToString("zzz");
    //  ************************************************
    //  ***** Optional ready_timestamp calculation *****
    DateTime tomorrow = DateTime.Now.AddDays(1);
    DateTime tomorrowTenAM = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 10, 0, 0, 0);
    estimateParams.utc_offset = tomorrowTenAM.ToString("zzz");
    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    TimeSpan diff = tomorrowTenAM.ToUniversalTime() - origin;
    estimateParams.ready_timestamp = (Int32)Math.Floor(diff.TotalSeconds);
    //  ***** End optional ready_timestamp calculation *****
    //  ****************************************************
    JObject estimate = brawndo.order.Estimate(estimateParams);

* **origin** - the origin (aka the pickup location) of the order.  Required.
* **destination** - the destination (aka the delivery location) of the order.  Required.
* **utc_offset** - the utc offset of the timezone where the order is taking place.  Required.
* **ready_timestamp** - the unix timestamp (in seconds) representing when the order is ready to be picked up.  If not set we assume immediate availability for pickup.
* **company_id** - if you are using brawndo as an enterprise client that manages other dropoff clients you can specify the managed client id who's estimate you want here.  This is optional and only works for enterprise clients.


An example of a successful response will look like this:

    {
        success : true,
        timestamp: '2015-03-05T14:51:14+00:00',
        data : {
            ETA: '243.1',
            Distance: '0.62',
            From: '78701',
            To: '78701',
            asap: {
                Price: '19.00',
                ETA: '243.1',
                Distance: '0.62'
            },
            two_hr: {
                Price: '17.00',
                ETA: '243.1',
                Distance: '0.62'
            },
            four_hr: {
                Price: '15.00',
                ETA: '243.1',
                Distance: '0.62'
            },
            all_day: {
                Price: '13.00',
                ETA: '243.1',
                Distance: '0.62'
            }
            service_type : 'standard'
        }
    }

* **data** - contain the pricing information for the allowed delivery window based on the given ready time, so you will not always see every option.
* **Distance** - the distance from the origin to the destination.
* **ETA** - the estimated time (in seconds) it will take to go from the origin to the destination.
* **From** - the origin zip code.  Only available if you have a zip to zip rate card configured.
* **To** - the destination zip code.  Only available if you have a zip to zip rate card configured.
* **asap** - the pricing for an order that needs to delivered within an hour of the ready time.
* **two_hr** - the pricing for an order that needs to delivered within two hours of the ready time.
* **four_hr** - the pricing for an order that needs to delivered within four hours of the ready time.
* **all_day** - the pricing for an order that needs to delivered by end of business on a weekday.
* **service_type** - The service type for pricing, could be standard, holiday, or after_hr.

### Placing an order <a id="placing"></a>

Given a successful estimate call, and a time window that you like, an order can be placed.  An order requires origin information, destination information, and specifics about the order.

#### Origin and Destination data.

The origin and destination contain information regarding the addresses in the order.

    OrderCreateParameters orderCreateParams = new OrderCreateParameters();
    
    orderCreateParams.origin = new OrderCreateAddress();
    orderCreateParams.origin.company_name = "Gus's Fried Chicken";
    orderCreateParams.origin.first_name = "Napoleon";
    orderCreateParams.origin.last_name = "Bonner";
    orderCreateParams.origin.address_line_1 = "117 San Jacinto Blvd";
    //orderCreateParams.origin.address_line_2 = "";
    orderCreateParams.origin.city = "Austin";
    orderCreateParams.origin.state = "TX";
    orderCreateParams.origin.zip = "78701";
    orderCreateParams.origin.phone = "5125555555";
    orderCreateParams.origin.email = "cluckcluck@gusfriedchicken.com";
    orderCreateParams.origin.lat = 30.263706;
    orderCreateParams.origin.lng = -97.741703;
    //orderCreateParams.origin.remarks = "";
    
    orderCreateParams.destination = new OrderCreateAddress();
    orderCreateParams.destination.company_name = "Dropoff";
    orderCreateParams.destination.first_name = "Algis";
    orderCreateParams.destination.last_name = "Woss";
    orderCreateParams.destination.address_line_1 = "901 S MoPac Expy";
    orderCreateParams.destination.address_line_2 = "#150";
    orderCreateParams.destination.city = "Austin";
    orderCreateParams.destination.state = "TX";
    orderCreateParams.destination.zip = "78746";
    orderCreateParams.destination.phone = "512-555-5555";
    orderCreateParams.destination.email = "awoss@dropoff.com";
    orderCreateParams.destination.lat = 30.264573;
    orderCreateParams.destination.lng = -97.782073;
    orderCreateParams.destination.remarks = "Please use the front entrance.  The back one is guarded by cats!";

* **address\_line\_1** - the street information for the origin or destination.  Required.
* **address\_line\_2** - additional information for the address for the origin or destination (ie suite number).  Optional.
* **company_name** - the name of the business for the origin or destination.  Required.
* **first_name** -  the first name of the contact at the origin or destination.  Required.
* **last_name** - the last name of the contact at the origin or destination.  Required.
* **phone** -  the contact number at the origin or destination.  Required.
* **email** -  the email address for the origin or destination.  Required.
* **city** -  the city for the origin or destination.  Required.
* **state** -  the state for the origin or destination.  Required.
* **zip** -  the zip code for the origin or destination.  Required.
* **lat** -  the latitude for the origin or destination.  Required.
* **lng** -  the longitude for the origin or destination.  Required.
* **remarks** -  additional instructions for the origin or destination.  Optional.

#### Order details data.

The details contain attributes about the order

    orderCreateParams.details = new OrderCreateDetails();
    orderCreateParams.details.ready_date = (Int32)Math.Floor(diff.TotalSeconds);
    orderCreateParams.details.type = "two_hr";
    orderCreateParams.details.quantity = 10;
    orderCreateParams.details.weight = 20;
    //  We are using the pricing for the two_hr time frame 
    //  for the estimate result we called earlier
    orderCreateParams.details.distance = (string) estimate["data"]["two_hr"]["Distance"];
    orderCreateParams.details.eta = (string) estimate["data"]["two_hr"]["ETA"];
    orderCreateParams.details.price = (string)estimate["data"]["two_hr"]["Price"];

* **quantity** - the number of packages in the order. Required.
* **weight** - the weight of the packages in the order.  **A heavier order could be subject to a price adjustment.** Required.
* **eta** - the eta from the origin to the destination.  Should use the value retrieved in the getEstimate call. Required.
* **distance** - the distance from the origin to the destination.  Should use the value retrieved in the getEstimate call. Required.
* **price** - the price for the order.  Should use the value retrieved in the getEstimate call. Required.
* **ready_date** - the unix timestamp (seconds) indicating when the order can be picked up. Can be up to 60 days into the future.  Required.
* **type** - the order window.  Can be asap, two_hr, four_hr, after_hr, or holiday depending on the ready_date. Required.
* **reference_name** - a field for your internal referencing. Optional.
* **reference_code** - a field for your internal referencing. Optional.

Once this data is created, you can create the order.

    JObject createResponse = brawndo.order.Create(orderCreateParams);

Note that if you want to create this order on behalf of a managed client as an enterprise client user you will need to specify the company_id.

    orderCreateParams.company_id = "1111111111111";
    JObject createResponse = brawndo.order.Create(orderCreateParams);

The data in the callback will contain the id of the new order as well as the url where you can track the order progress.
    
    string created_order_id = (string)createResponse["data"]["order_id"];
    string created_order_url = (string)createResponse["data"]["url"];


### Cancelling an order <a id="cancel"></a>

    OrderCancelParameters cancelParameters = new OrderCancelParameters();
    cancelParameters.order_id = created_order_id;
    JObject cancelResult = brawndo.order.Cancel();

	
If you are trying to cancel an order for a manage client order as an enterprise client user, include the company_id in the argument parameters

	 OrderCancelParameters cancelParameters = new OrderCancelParameters();
    cancelParameters.order_id = created_order_id;
    cancelParameters.company_id = "1111111111111";
    JObject cancelResult = brawndo.order.Cancel();
    
* **order_id** - the id of the order to cancel.
* **company_id** - if you are using brawndo as an enterprise client that manages other dropoff clients you can specify the managed client id who you would like to cancel an order for. This is optional and only works for enterprise clients.

An order can be cancelled in these situations

* The order was placed less than **ten minutes** ago.
* The order ready time is more than **one hour** away.
* The order has not been picked up.
* The order has not been cancelled.

An example of a succesful cancel result is:

    {
        "success": true,
        "timestamp": "2017-06-06T13:15:32Z"
    }

    
### Getting a specific order <a id="specific"></a>

    OrderGetParameters orderGetParams = new OrderGetParameters();
    orderGetParams.order_id = "zzzz-zzzz-zzz";

Example response

    {
         data: {
             destination: {
                 order_id: 'ac156e24a24484a382f66b8cadf6fa83',
                 short_id: '06ex-r3zV-BMb',
                 createdate: 1425653646,
                 updatedate: 1425653646,
                 order_status_code: 0,
                 company_name: 'Dropoff Inc.',
                 first_name: 'Algis',
                 last_name: 'Woss',
                 address_line_1: '800 Brazos Street',
                 address_line_2: '250',
                 city: 'Austin',
                 state: 'TX',
                 zip: '78701',
                 phone_number: '8444376763',
                 email_address: 'deliveries@dropoff.com',
                 lng: -97.740838,
                 lat: 30.269967
             },
             details: {
                 order_id: 'ac156e24a24484a382f66b8cadf6fa83',
                 short_id: '06ex-r3zV-BMb',
                 createdate: 1425653646,
                 customer_name: 'Algis Woss',
                 type: 'ASAP',
                 market: 'austin',
                 timezone: 'America/Chicago',
                 price: '15.00',
                 signed: 'false',
                 distance: '0.62',
                 order_status_code: 0,
                 wait_time: 0,
                 order_status_name: 'Submitted',
                 pickupETA: 'TBD',
                 deliveryETA: '243.1',
                 signature_exists: 'NO',
                 quantity: 1,
                 weight: 5,
                 readyforpickupdate: 1425578400,
                 updatedate: 1425653646
             },
             origin: {
                 order_id: 'ac156e24a24484a382f66b8cadf6fa83',
                 short_id: '06ex-r3zV-BMb',
                 createdate: 1425653646,
                 updatedate: 1425653646,
                 order_status_code: 0,
                 company_name: 'Gus's Fried Chicken',
                 first_name: 'Napoleon',
                 last_name: 'Bonner',
                 address_line_1: '117 San Jacinto Blvd',
                 city: 'Austin',
                 state: 'TX',
                 zip: '78701',
                 phone_number: '5124744877',
                 email_address: 'orders@gussfriedchicken.com',
                 lng: -97.741703,
                 lat: 30.263706,
                 market: 'austin',
                 remarks: 'Be nice to napoleon'
             }
        },
        success: true,
        timestamp: '2015-03-09T18:42:15+00:00'
    }

### Getting a page of orders <a id="page"></a>

Get the first page of orders

    OrderGetParameters orderGetParams = new OrderGetParameters();
    JObject page = brawndo.order.Get(orderGetParams);

Get a page of orders after the last_key from a previous response

    OrderGetParameters nextPageParams = new OrderGetParameters();
    string page1LastKey = (string)page["last_key"];
    
    if (page["last_key"] != null)
    {
        nextPageParams.last_key = (string) page["last_key"];
    }

    JObject nextPage = brawndo.order.Get(nextPageParams);

Get the first page of orders as an enterprise client user for a managed client


    OrderGetParameters orderGetParams = new OrderGetParameters();
    orderGetParams.company_id = "1111111111111";
    JObject page = brawndo.order.Get(orderGetParams);
    
Get a page of orders after the last_key from a previous response as an enterprise client user for a managed client

    OrderGetParameters nextPageParams = new OrderGetParameters();
    nextPageParams.company_id = "1111111111111";
    
    string page1LastKey = (string)page["last_key"];
    
    if (page["last_key"] != null)
    {
        nextPageParams.last_key = (string) page["last_key"];
    }

    JObject nextPage = brawndo.order.Get(nextPageParams);

Example response

    {
        data: [ ... ],
        count: 10,
        total: 248,
        last_key: 'zhjklzvxchjladfshjklafdsknvjklfadjlhafdsjlkavdnjlvadslnjkdas',
        success: true,
        timestamp: '2015-03-09T18:42:15+00:00'
    }

## Tips <a id="tips"></a>

You can create, delete, and read tips for individual orders.  Please note that tips can only be created or deleted for orders that were delivered within the current billing period.  Tips are paid out to our agents and will appear as an order adjustment charge on your invoice after the current billing period has expired.  Tip amounts must not be zero or negative.  You are limited to one tip per order.

### Creating a tip <a id="tip_create"></a>

Tip creation requires two parameters, the order id **(order_id)** and the tip amount **(amount)**.

    TipParameters tipParameters = new TipParameters();
    tipParameters.order_id = created_order_id;
    tipParameters.amount = 4.44;
    JObject tipResponse = brawndo.order.tip.Create(tipParameters);

### Deleting a tip <a id="tip_delete"></a>

Tip deletion only requires the order id **(order_id)**.

    TipParameters tipParameters = new TipParameters();
    tipParameters.order_id = created_order_id;
    JObject tipResponse = brawndo.order.tip.Delete(tipParameters);
	
If you are trying to delete a tip on a managed client order as an enterprise client user, include the company_id in the argument parameters

    TipParameters tipParameters = new TipParameters();
    tipParameters.order_id = created_order_id;
    tipParameters.company_id = "1111111111111";
    JObject tipResponse = brawndo.order.tip.Delete(tipParameters);

### Reading a tip <a id="tip_read"></a>

Tip reading only requires the order id **(order_id)**.

    TipParameters tipParameters = new TipParameters();
    tipParameters.order_id = created_order_id;
    JObject tipResposne = brawndo.order.Get(tipParameters);
	
If you are trying to read a tip on a manage client order as an enterprise client user, include the company_id in the argument parameters

    TipParameters tipParameters = new TipParameters();
    tipParameters.order_id = created_order_id;
    tipParameters.company_id = "1111111111111";
    JObject tipResposne = brawndo.order.Get(tipParameters);

Example response:

	{
		amount: "4.44"
		createdate: "2016-02-18T16:46:52+00:00"
		description: "Tip added by Dropoff(Algis Woss)"
		updatedate: "2016-02-18T16:46:52+00:00"
	}

## Webhooks <a id="webhook"></a>

You may register a server route with Dropoff to receive real time updates related to your orders.

Your endpoint must handle a post, and should verify the X-Dropoff-Key with the client key given to you when registering the endpoint.

The body of the post should be signed using the HMAC-SHA-512 hashing algorithm combined with the client secret give to you when registering the endpoint.

The format of a post from Dropoff will be:

    {
        count : 2,
        data : [ ]
    }

* **count** contains the number of items in the data array.
* **data** is an array of events regarding orders and agents processing those orders.

### Backoff algorithm <a id="backoff"></a>

If your endpoint is unavailable Dropoff will try to resend the events in this manner:

*  Retry 1 after 10 seconds
*  Retry 2 after twenty seconds
*  Retry 3 after thirty seconds
*  Retry 4 after one minute
*  Retry 5 after five minutes
*  Retry 6 after ten minutes
*  Retry 7 after fifteen minutes
*  Retry 8 after twenty minutes
*  Retry 9 after thirty minutes
*  Retry 10 after forty five minutes
*  All subsequent retries will be after one hour until 24 hours have passed

**If all retries have failed then the cached events will be forever gone from this plane of existence.**

### Events <a id="events"></a>

There are two types of events that your webhook will receive, order update events and agent location events.

All events follow this structure:

    {
        event_name : <the name of the event ORDER_UPDATED or AGENT_LOCATION>
        data : { ... }
    }

* **event_name** is either **ORDER_UPDATED** or **AGENT_LOCATION**
* **data** contains the event specific information

#### Order Update Event

This event will be triggered when the order is either:

* Accepted by an agent.
* Picked up by an agent.
* Delivered by an agent.
* Cancelled.

This is an example of an order update event

    {
        event_name: 'ORDER_UPDATED',
        data: {
            order_status_code: 1000,
            company_id: '7df2b0bdb418157609c0d5766fb7fb12',
            timestamp: '2015-05-15T12:52:55+00:00',
            order_id: 'klAb-zwm8-mYz',
            agent_id: 'b7aa983243ccbfa43410888dd205c298'
        }
    }

* **order_status_code** can be -1000 (cancelled), 1000 (accepted), 2000 (picked up), or 3000 (delivered)
* **company_id** is your company id.
* **timestamp** is a utc timestamp of when the order occured.
* **order_id** is the id of the order.
* **agent_id** is the id of the agent that is carrying out your order.

#### Agent Location Update Event

This event is triggered when the location of an agent that is carrying out your order has changed.

    {
        event_name: 'AGENT_LOCATION',
        data: {
            agent_avatar: 'https://s3.amazonaws.com/com.dropoff.alpha.app.workerphoto/b7aa983243ccbfa43410888dd205c298/worker_photo.png?AWSAccessKeyId=AKIAJN2ULWKTZXXEOQDA&Expires=1431695270&Signature=AFKNQdT33lhlEddrGp0kINAR4uw%3D',
            latitude: 30.2640713,
            longitude: -97.7469492,
            order_id: 'klAb-zwm8-mYz',
            timestamp: '2015-05-15T12:52:50+00:00',
            agent_id: 'b7aa983243ccbfa43410888dd205c298'
        }
    }

* **agent_avatar** is an image url you can use to show the agent.  It expires in 15 minutes.
* **latitude** and **longitude** reflect the new coordinates of the agent.
* **timestamp** is a utc timestamp of when the order occured.
* **order_id** is the id of the order.
* **agent_id** is the id of the agent that is carrying out your order.


#### Managed Client Events<a id="managed_client_events"></a>

If you have registered a webhook with an enterprise client that can manager other clients, then the webhook will also receive all events for any managed clients.

So in our hierarchical [example](#managed_clients) at the start, if a webhook was registered for **EnterpriseCo Global**, it would receive all events for:

- EnterpriseCo Global
- EnterpriseCo Europe
- EnterpriseCo Paris
- EnterpriseCo London
- EnterpriseCo Milan
- EnterpriseCo NA
- EnterpriseCo Chicago
- EnterpriseCo New York
- EnterpriseCo Los Angeles


### Simulating an order<a id="simulation"></a>

You can simulate an order via the brawndo api in order to test your webhooks.

The simulation will create an order, assign it to a simulation agent, and move the agent from pickup to the destination.

**You can only run a simulation once every fifteen minutes, and only in the sandbox.**

    brawndo.order.Simulate("austin");
