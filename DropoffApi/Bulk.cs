using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dropoff
{
  public struct BulkCreateParams
  {
    public string filename;
    public string company_id;
  }

  public struct BulkCancelParams
  {
    public string bulk_id;
  }

  public class Bulk
  {
    private Client client;

    internal Bulk(Client client)
    {
      this.client = client;
    }
    public JObject Create(BulkCreateParams parameters)
    {
      Dictionary<string, string> query = new Dictionary<string, string>();

      if (parameters.company_id != null)
      {
          query.Add("company_id", parameters.company_id);
      }

      string filename = parameters.filename;

      JObject result = client.DoPost("/bulkupload", "bulkupload", filename, query);
      return result;
    }
  }
}