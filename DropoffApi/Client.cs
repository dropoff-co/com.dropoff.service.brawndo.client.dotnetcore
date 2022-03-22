using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace Dropoff
{
    class Client
    {
        private string apiUrl;
        private string host;
        private string privateKey;
        private string publicKey;
        private HttpClient client;

        public Client(string apiUrl, string host, string privateKey, string publicKey)
        {
            this.apiUrl = apiUrl;
            this.host = host;
            this.privateKey = privateKey;
            this.publicKey = publicKey;
            this.client = new HttpClient() {
                BaseAddress = new Uri(apiUrl)
            };
        }

        private string GetXDropoffDate()
        {
            DateTime now = DateTime.UtcNow;
            return now.ToString("yyyyMMddTHHmmssZ");
        }

        private string ToHex(byte[] bytes)
        {
            string hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "");
        }

        private string DoHMAC(string input, string key)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);
            byte[] inputBytes = encoding.GetBytes(input);
            HMACSHA512 sha512 = new HMACSHA512(keyBytes);

            byte[] hashMessage = sha512.ComputeHash(inputBytes);
            return ToHex(hashMessage).ToLower();
        }

        private async Task<JObject> DoRequest(HttpMethod method, string path, string resource, IDictionary<string, string> query, string payload)
        {
            string x_dropoff_date = this.GetXDropoffDate();

            UriBuilder uri = new UriBuilder(apiUrl + path);
            if (query != null)
            {
                System.Collections.ArrayList queries = new System.Collections.ArrayList();
                foreach (KeyValuePair<string,string> pair in query)
                {
                    string encode = System.Net.WebUtility.UrlEncode(pair.Value);
                    queries.Add(pair.Key + "=" + System.Net.WebUtility.UrlEncode(pair.Value));
                }

                if (queries.Count > 0)
                {
                    uri.Query = string.Join("&", (string[])queries.ToArray(Type.GetType("System.String")));
                }
            }

            HttpRequestMessage message = new HttpRequestMessage(method, uri.Uri);
            message.Headers.Add("X-Dropoff-Date", x_dropoff_date);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Host = this.host;
            ProductInfoHeaderValue productInfo = new ProductInfoHeaderValue(new ProductHeaderValue("DropoffBrawndo","1.0"));
            message.Headers.UserAgent.Add(productInfo);

            if (payload != null && resource == "bulkupload") 
            {
                FileStream fs = new FileStream(payload, FileMode.Open, FileAccess.Read);
                byte[] fileContent = new byte[fs.Length];
                fs.Read(fileContent, 0, fileContent.Length);
                fs.Close();
                var fileStream = new MultipartFormDataContent();
                fileStream.Add(new ByteArrayContent(fileContent, 0, fileContent.Length), "file", payload);
                message.Content = fileStream;

            }
            else if (payload != null)
            {
                message.Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            }

            string headerString = "";
            string headerKeyString = "";
            string authorizationBody = "";

            System.Collections.SortedList sortedHeaders = new System.Collections.SortedList();

            foreach (KeyValuePair<String, IEnumerable<String>> header in message.Headers)
            {
                sortedHeaders.Add(header.Key.ToLower(), header.Value);
            }

            foreach (System.Collections.DictionaryEntry header in sortedHeaders)
            {
                IEnumerable<String> values = (IEnumerable<String>)header.Value;

                if (headerString != "")
                {
                    headerString += "\n";
                    headerKeyString += ";";
                }

                string valuesString = "";

                foreach (string value in values)
                {
                    if (valuesString != "")
                    {
                        valuesString += ",";
                    }
                    valuesString += value;
                }
                headerString += header.Key + ":" + valuesString;
                headerKeyString += header.Key;
            }

            if (headerString != "")
            {
                headerString += "\n";
            }

            authorizationBody += method.ToString() + "\n";
            authorizationBody += path + "\n\n";
            authorizationBody += headerString + "\n";
            authorizationBody += headerKeyString + "\n";

            string bodyHash = this.DoHMAC(authorizationBody, this.privateKey);

            string finalStringToHash = "HMAC-SHA512\n" + x_dropoff_date + "\n" + resource + "\n" + bodyHash;

            string firstKey = "dropoff" + this.privateKey;
            string finalHash = this.DoHMAC(x_dropoff_date.Substring(0, 8), firstKey);
            finalHash = this.DoHMAC(resource, finalHash);
            string authHash = this.DoHMAC(finalStringToHash, finalHash);

            message.Headers.TryAddWithoutValidation("Authorization", "Authorization: HMAC-SHA512 Credential=" + this.publicKey);
            message.Headers.TryAddWithoutValidation("Authorization", "SignedHeaders=" + headerKeyString);
            message.Headers.TryAddWithoutValidation("Authorization", "Signature=" + authHash);

            HttpResponseMessage response = this.client.SendAsync(message).Result;

            string content = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(content);
            return await Task.Run(() => data);
        }

        public JObject DoGet(string path, string resource, IDictionary<string, string> query)
        {
            Task<JObject> task = Task.Run(async () => await this.DoRequest(HttpMethod.Get, path, resource, query, null));
            task.Wait();
            return task.Result;
        }

        public JObject DoPost(string path, string resource, string payload, IDictionary<string, string> query)
        {
            Task<JObject> task = Task.Run(async () => await this.DoRequest(HttpMethod.Post, path, resource, query, payload));
            task.Wait();
            return task.Result;
        }

        public JObject DoDelete(string path, string resource, IDictionary<string, string> query)
        {
            Task<JObject> task = Task.Run(async () => await this.DoRequest(HttpMethod.Delete, path, resource, query, null));
            task.Wait();
            return task.Result;
        }
        public JObject DoPut(string path, string resource, string payload, IDictionary<string, string> query)
        {
            Task<JObject> task = Task.Run(async () => await this.DoRequest(HttpMethod.Put, path, resource, query, null));
            task.Wait();
            return task.Result;
        }
    }
}
