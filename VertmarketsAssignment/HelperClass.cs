using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VertmarketsAssignment
{
    public class HelperClass
    {
        SharedClass shared;

        public HelperClass(SharedClass shared)
        {
            this.shared = shared;
        }
        public async Task<string> GetAuthorizeToken()
        {
            Console.WriteLine("Inside GetAuthorizeToken to get Token Id");
            string tokenId = string.Empty;           
            var response = await ApiServiceCall(string.Concat(this.shared.Url, "/api/token"));
            if(!string.IsNullOrEmpty(response))
            {
                var data = JObject.Parse(response);
                tokenId = data["token"].ToString();
            }
            else
            {
                Console.WriteLine("ApiServiceCall return empty Json Object");
            }
            return tokenId;
        }

        public async Task<Categories> GetCategories(string token)
        {
            Console.WriteLine("Inside GetCategories Method");
            Categories catgs = new Categories();
            catgs.categories = new List<string>();
            List<string> categories = new List<string>();
            var response = await ApiServiceCall(string.Concat(this.shared.Url, "/api/categories/",token));
            if (!string.IsNullOrEmpty(response))
            {
                dynamic data = JObject.Parse(response);
                categories = data["data"].ToObject<List<string>>();
                foreach(var v in categories)
                {
                    catgs.categories.Add(v);
                }
            }
            else
            {
                Console.WriteLine("ApiServiceCall return empty Json Object");
            }
            return catgs;
        }

        public async Task<Dictionary<string, List<int>>> GetMagazinesData(string token, Categories categories)
        {
            Console.WriteLine("Inside GetMagazinesData Method");
            Dictionary<string, List<int>> magzCatDic = new Dictionary<string, List<int>>();
            List<int> magIds = new List<int>();
            Magazines magazines = new Magazines();
            foreach (var cat in categories.categories)
            {
                var response = await ApiServiceCall(string.Concat(this.shared.Url, "/api/magazines/", token, "/", cat));
                if (!string.IsNullOrEmpty(response))
                {
                    magIds = new List<int>();
                    magazines = JsonConvert.DeserializeObject<Magazines>(response);
                    foreach (var mag in magazines.data)
                    {
                        magIds.Add(mag.id);
                    }
                }
                else
                {
                    Console.WriteLine("ApiServiceCall return empty Json Object");
                }
                magzCatDic.Add(cat, magIds);
            }

            return magzCatDic;
        }
        public async Task<Subscribers> GetSubscribersData(string token, Dictionary<string, List<int>> dictData,Categories categories)
        {
            Console.WriteLine("Inside GetSubscribersData Method");
            List<int> subscribersMagIDs = new List<int>();
            Subscribers subscibers = new Subscribers();
            Subscribers mainSub = new Subscribers();
            bool IsSubscribed = true;
            List<SubscribersData> mainSubscribersData = new List<SubscribersData>();
            var response = await ApiServiceCall(string.Concat(this.shared.Url, "/api/subscribers/", token));
            if (!string.IsNullOrEmpty(response))
            {
                subscibers = JsonConvert.DeserializeObject<Subscribers>(response);
            }
            else
            {
                Console.WriteLine("ApiServiceCall return empty Json Object");
            }
            
            var subData = subscibers.data.ToList();
            foreach (var data in subData)
            {
                foreach (var catg in categories.categories)
                {
                    var magIds = dictData[catg];

                    if (data.magazineIds.ToList().Intersect(magIds).Any())
                    {
                        IsSubscribed = true;
                    }
                    else
                    {
                        IsSubscribed = false;
                        break;
                    }
                }
                if(IsSubscribed)
                {
                    mainSubscribersData.Add(data);
                    mainSub.data = mainSubscribersData.ToArray();
                }
            }
            return mainSub;
        }

        public async Task<string> PostAnswer(List<string> iDs,string token)
        {
            //string token = await GetAuthorizeToken();
            string responseString = string.Empty;
            string formatString = string.Empty;
            string content = string.Empty;
            foreach(var s in iDs)
            {
                formatString = formatString + "," + "\"" + s + "\"";
            }
            formatString = formatString.TrimStart(',');
            using (HttpClient client = new HttpClient())
            {
                content = string.Format("{{\"subscribers\":[{0}]}}", formatString);
              HttpContent Content = new StringContent(content, Encoding.UTF8, "application/json");
              HttpResponseMessage response = await client.PostAsync(string.Concat(this.shared.Url, "/api/answer/", token), Content).ConfigureAwait(false);
                responseString = await response.Content.ReadAsStringAsync();
            }

                
            return responseString;
        }
        private async Task<string> ApiServiceCall(string url)
        {
            string responseObj = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = new HttpResponseMessage();
                response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    responseObj = await response.Content.ReadAsStringAsync();
                    return responseObj;
                }
                else
                {
                    Console.WriteLine("Service Call Failed!!!");
                }
            }
            return responseObj;
        }
        
    }
}
