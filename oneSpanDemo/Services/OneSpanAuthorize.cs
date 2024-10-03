using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using oneSpanDemo.Models;
using System.Text;
using System.Transactions;

namespace oneSpanDemo.Services
{
    public class OneSpanAuthorize :IoneSpanAuthorize
    {
        private IConfiguration configuration;
        private readonly string sandBoxUrl;
        private readonly string oneSpanClientId;
        private readonly string oneSpanClientSecret;

        public OneSpanAuthorize(IConfiguration _iconfig)
        {
            configuration = _iconfig;
            sandBoxUrl = configuration.GetValue<string>("OneSpanSandboxUrl");
            oneSpanClientId = configuration.GetValue<string>("OneSpanClientId");
            oneSpanClientSecret = configuration.GetValue<string>("OneSpanSecret");
        }
        public string GetAccessToken()
        {
            string token = string.Empty;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, sandBoxUrl + "apitoken/clientApp/accessToken");
                    requestMessage.Content = new StringContent("{\"clientId\":\"" + oneSpanClientId + "\",\"secret\":\"" + oneSpanClientSecret + "\",\"type\": \"OWNER\"}", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;

                    string httpResult = response.Content.ReadAsStringAsync().Result;

                    JObject json = JObject.Parse(httpResult);
                    if (json.HasValues)
                    {
                        JToken? stringAccessToken = json.GetValue("accessToken");
                        if (stringAccessToken != null)
                        {
                            token = stringAccessToken.ToString();
                        }
                    }
                }
                catch (Exception ex) { 
                // TODO- Logging
                }
            }
            return token;
        }
    }
}
