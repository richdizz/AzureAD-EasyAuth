using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DX.AzureAD.EasyOAuth.Utils
{
    public static class AppRegistration
    {
        public static Dictionary<string, string> UpdateWebAppRegistration(string token, string tenantId, string appName,
            string signOnUri, string appIdUri, string redirectUri, bool multiTenant, bool twoYearSecret, List<Scopes> appScopes,
            string existingAppId)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            var registrationManifest = new JObject(
                                            new JProperty("availableToOtherTenants", multiTenant ? "True" : "False"),
                                            new JProperty("displayName", appName),
                                            new JProperty("homePage", signOnUri),
                                            new JProperty("identifierUris",
                                                new JArray(
                                                    new JValue(appIdUri)
                                                    )
                                                ),
                                            new JProperty("replyUrls",
                                                new JArray(
                                                    new JValue(redirectUri)
                                                    )
                                                ),
                                            new JProperty("requiredResourceAccess", GenerateRequiredAccess(appScopes.ToArray()))
                                            );

            string payload = JsonConvert.SerializeObject(registrationManifest);

            string result = UpdateAppRegistration(token, tenantId, payload, existingAppId);
            if (result.StartsWith("ERROR"))
            {
                // Check for expired token and handle specially
                // Implementing refresh can come later.
                if (result.Contains("Authentication_ExpiredToken"))
                {
                    //results.Add(Utils.Constants.errorMessageTagStr, "Your session has expired. Please logout and log in again.");
                }
                else
                {
                    //results.Add(Utils.Constants.errorMessageTagStr, result);
                }
            }

            return results;
        }
        public static Dictionary<string, string> CreateWebAppRegistration(string token, string tenantId, string appName,
            string signOnUri, string appIdUri, string redirectUri, bool multiTenant, bool twoYearSecret, List<Scopes> appScopes)
        {
            Dictionary<string, string> registrationInfo = new Dictionary<string, string>()
            {
                { "client_id", "" },
                { "client_secret", GenerateAppSecret() },
                { "error_message", "" },
            };

            var registrationManifest = new JObject(
                                            new JProperty("objectType", "Application"),
                                            new JProperty("availableToOtherTenants", multiTenant ? "True" : "False"),
                                            new JProperty("displayName", appName),
                                            new JProperty("homePage", signOnUri),
                                            new JProperty("oauth2AllowImplicitFlow", "True"),
                                            new JProperty("identifierUris",
                                                new JArray(
                                                    new JValue(appIdUri)
                                                    )
                                                ),
                                            new JProperty("passwordCredentials",
                                                new JArray(
                                                    GeneratePasswordCredential(registrationInfo["client_secret"], twoYearSecret)
                                                    )
                                                ),
                                            new JProperty("replyUrls",
                                                new JArray(
                                                    new JValue(redirectUri)
                                                    )
                                                ),
                                            new JProperty("requiredResourceAccess", GenerateRequiredAccess(appScopes.ToArray()))
                                            );

            string payload = JsonConvert.SerializeObject(registrationManifest);

            string result = CreateAppRegistration(token, tenantId, payload);
            if (result.StartsWith("ERROR"))
            {
                // Check for expired token and handle specially
                // Implementing refresh can come later.
                if (result.Contains("Authentication_ExpiredToken"))
                {
                    registrationInfo["error_message"] = "Your session has been expired. Please logout and log in again.";
                }
                else if (result.Contains("Authorization_RequestDenied"))
                {
                    registrationInfo["error_message"] = "Request Denied !! Please sign in again using an account that has access to resources.";
                }
                else
                {
                    registrationInfo["error_message"] = result;
                }
            }
            else
            {
                registrationInfo["client_id"] = result;
            }

            return registrationInfo;
        }

        private static string CreateAppRegistration(string token, string tenantId, string payload)
        {
            string graphEndpoint = string.Format("https://graph.windows.net/{0}/applications?api-version=1.5", tenantId);
            using (var client = new HttpClient())
            {
                var content = new StringContent(payload);
                content.Headers.ContentType.MediaType = "application/json";
                var request = new HttpRequestMessage(HttpMethod.Post, graphEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OutlookDevPortal", "1.0"));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = content;

                var result = client.SendAsync(request).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    try
                    {
                        dynamic appRegistration = JsonConvert.DeserializeObject(resultContent);

                        string appId = appRegistration["appId"];
                        return appId;
                    }
                    catch (JsonException ex)
                    {
                        return string.Format("ERROR: {0}", ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        dynamic errorDetails = JsonConvert.DeserializeObject(resultContent);
                        return string.Format("ERROR: {0} - {1}", errorDetails["odata.error"]["code"],
                            errorDetails["odata.error"]["message"]["value"]);

                    }
                    catch (JsonException)
                    {
                        return string.Format("ERROR: Request returned {0}", result.StatusCode);
                    }
                }
            }
        }

        private static string GetErrorDetails(string response, System.Net.HttpStatusCode code)
        {
            try
            {
                dynamic errorDetails = JsonConvert.DeserializeObject(response);
                return string.Format("ERROR: {0} - {1}", errorDetails["odata.error"]["code"],
                     errorDetails["odata.error"]["message"]["value"]);
            }
            catch (JsonException)
            {
                return string.Format("ERROR: Request returned {0}", code);
            }
        }
        private static string UpdateAppRegistration(string token, string tenantId, string payload, string appId)
        {
            #region Get applicaiton object
            string appEndPoint = string.Format("https://graph.windows.net/{0}/applications?$filter=appId eq '{1}' &api-version=1.5", tenantId, appId);
            string appidObject = null;
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, appEndPoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OutlookDevPortal", "1.0"));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = client.SendAsync(request).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    return GetErrorDetails(resultContent, result.StatusCode);
                }
                dynamic appObject = (JObject.Parse(resultContent))["value"];
                appidObject = appObject[0].objectId;
            }
            #endregion
            #region Update the app info

            string graphEndpoint = string.Format("https://graph.windows.net/{0}/applications/{1}?api-version=1.5", tenantId, appidObject);
            using (var client = new HttpClient())
            {
                var content = new StringContent(payload);
                var method = new HttpMethod("PATCH");
                content.Headers.ContentType.MediaType = "application/json";
                var request = new HttpRequestMessage(method, graphEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OutlookDevPortal", "1.0"));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = content;

                var result = client.SendAsync(request).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    return resultContent; /* on success result content is empty string*/
                }
                else
                {
                    return GetErrorDetails(resultContent, result.StatusCode);
                }
            }
            #endregion
        }

        private static string GenerateAppSecret()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] buff = new byte[32];
            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        private static JObject GeneratePasswordCredential(string secret, bool twoYear)
        {
            DateTime validStart = DateTime.UtcNow;
            DateTime validEnd = validStart.AddYears(twoYear ? 2 : 1);
            var credential = new JObject(
                                new JProperty("startDate", validStart.ToString("o")),
                                new JProperty("endDate", validEnd.ToString("o")),
                                new JProperty("value", secret)
                                );
            //{
            //    startDate = validStart.ToString("o"),
            //    endDate = validEnd.ToString("o"),
            //    value = secret
            //};

            return credential;
        }

        private static JArray GenerateRequiredAccess(Scopes[] appScopes)
        {
            var requiredAccess = new JArray();

            var graphAccess = GetResourceNode(Resources.AzureGraph, appScopes);
            if (graphAccess != null)
                requiredAccess.Add(graphAccess);

            var msftGraphResource = GetResourceNode(Resources.MicrosoftGraph, appScopes);
            if (msftGraphResource != null)
                requiredAccess.Add(msftGraphResource);

            return requiredAccess;
        }

        private static JObject GetResourceNode(Resources resource, Scopes[] appScopes)
        {
            var entries = new JArray();

            foreach (Scopes scope in appScopes)
            {
                if (AppScopes.GetScopeResource(scope) == resource)
                {
                    var entry = new JObject(
                                    new JProperty("id", AppScopes.ScopeIds[scope]),
                                    new JProperty("type", "Scope")
                                    );

                    entries.Add(entry);
                }
            }

            if (entries.Count > 0)
            {
                var node = new JObject(
                                new JProperty("resourceAppId", AppScopes.ResourceIds[resource]),
                                new JProperty("resourceAccess", entries)
                                );

                return node;
            }

            return null;
        }

        public static Dictionary<string, string> CreateNativeAppRegistration(string token, string tenantId,
            string appName, string redirectUri, List<Scopes> appScopes)
        {
            Dictionary<string, string> registrationInfo = new Dictionary<string, string>();

            var registrationManifest = new JObject(
                                            new JProperty("objectType", "Application"),
                                            new JProperty("availableToOtherTenants", "True"),
                                            new JProperty("publicClient", "True"),
                                            new JProperty("displayName", appName),
                                            new JProperty("replyUrls",
                                                new JArray(
                                                    new JValue(redirectUri)
                                                    )
                                                ),
                                            new JProperty("requiredResourceAccess", GenerateRequiredAccess(appScopes.ToArray()))
                                            );

            string payload = JsonConvert.SerializeObject(registrationManifest);

            string result = CreateAppRegistration(token, tenantId, payload);
            if (result.StartsWith("ERROR"))
            {
                // Check for expired token and handle specially
                // Implementing refresh can come later.
                if (result.Contains("Authentication_ExpiredToken"))
                {
                    registrationInfo.Add(Constants.errorMessageTagStr, "Your session has expired. Please logout and log in again.");
                }
                else
                {
                    registrationInfo.Add(Constants.errorMessageTagStr, result);
                }
            }
            else
            {
                registrationInfo.Add(Constants.clientIdTagStr, result);
            }

            return registrationInfo;
        }

        public static Dictionary<string, string> UpdateNativeAppRegistration(string token, string tenantId,
    string appName, string redirectUri, List<Scopes> appScopes, string existingAppId)
        {
            Dictionary<string, string> registrationInfo = new Dictionary<string, string>();

            var registrationManifest = new JObject(
                                            new JProperty("displayName", appName),
                                            new JProperty("replyUrls",
                                                new JArray(
                                                    new JValue(redirectUri)
                                                    )
                                                ),
                                            new JProperty("requiredResourceAccess", GenerateRequiredAccess(appScopes.ToArray()))
                                            );

            string payload = JsonConvert.SerializeObject(registrationManifest);

            string result = UpdateAppRegistration(token, tenantId, payload, existingAppId);
            if (result.StartsWith("ERROR"))
            {
                // Check for expired token and handle specially
                // Implementing refresh can come later.
                if (result.Contains("Authentication_ExpiredToken"))
                {
                    registrationInfo.Add(Constants.errorMessageTagStr, "Your session has expired. Please logout and log in again.");
                }
                else
                {
                    registrationInfo.Add(Constants.errorMessageTagStr, result);
                }
            }
            return registrationInfo;
        }
    }
}

