using DX.AzureAD.EasyOAuth.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DX.AzureAD.EasyOAuth.Utils
{
    public class TokenHelper
    {
        public async static Task<TokenModel> GetAccessTokenWithCode(string code, string resource, string clientId, string clientSecret, string socket)
        {
            //Retrieve access token using authorization code
            TokenModel token = null;
            HttpClient client = new HttpClient();
            string redirect = String.Format("https://{0}/OAuth/AuthCode/{1}/{2}/", HttpContext.Current.Request.Url.Authority, clientId, socket);
            HttpContent content = new StringContent(String.Format(@"grant_type=authorization_code&redirect_uri={0}&client_id={1}&client_secret={2}&code={3}&resource={4}", redirect, clientId, HttpUtility.UrlEncode(clientSecret), code, resource));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            using (HttpResponseMessage response = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/token", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<TokenModel>(json);
                }
            }
            return token;
        }
    }
}
