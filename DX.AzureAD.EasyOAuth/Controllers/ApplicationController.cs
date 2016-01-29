using DX.AzureAD.EasyOAuth.Models;
using DX.AzureAD.EasyOAuth.Utils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DX.AzureAD.EasyOAuth.Controllers
{
    public class ApplicationController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            //check for tenantId in session
            if (Session["TenantID"] == null)
                return RedirectToAction("Error", "Home", new { error = "Session expired" });
            var tenantId = Session["TenantID"].ToString();

            //get all registered apps for this tenant
            var apps = new List<ApplicationModel>();
            using (ApplicationEntities entities = new ApplicationEntities())
            {
                var tenantIdGuid = new Guid(tenantId);
                var regs = entities.Applications.Where(i => i.TenantId == tenantIdGuid);
                foreach (var reg in regs)
                {
                    var app = new ApplicationModel()
                    {
                        CliendId = reg.Id,
                        Name = reg.Name,
                        AppOriginsFlat = reg.Origins,
                    };
                    app.AppOrigins = app.AppOriginsFlat.Split(';').ToList();
                    apps.Add(app);
                }
            }
            return View(apps);
        }

        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            //check for tenantId and refresh token in session
            if (Session["TenantID"] == null || Session["RefreshToken"] == null)
                return RedirectToAction("Error", "Home", new { error = "Session expired" });
            var tenantId = Session["TenantID"].ToString();
            var refreshToken = Session["RefreshToken"].ToString();

            //use authentication context to get access token to azure graph
            AuthenticationContext context = new AuthenticationContext(string.Format("{0}/{1}", SettingsHelper.AuthorizationUri, tenantId));
            var result = await context.AcquireTokenByRefreshTokenAsync(refreshToken, new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret), SettingsHelper.AADGraphResourceId);

            ////delete the app in Azure
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
            //client.DefaultRequestHeaders.Add("Accept", "application/json; odata=verbose");
            //using (HttpResponseMessage response = await client.DeleteAsync(new Uri(string.Format("https://graph.windows.net/{0}/applications?$filter=appId eq '{1}'&api-version=1.5", tenantId, id.ToString()), UriKind.Absolute)))
            //{
            //    if (response.IsSuccessStatusCode)
            //    {
            //        //delete the app in the database
            //    }
            //}

            //delete the app in the database
            using (ApplicationEntities entities = new ApplicationEntities())
            {
                var item = entities.Applications.FirstOrDefault(i => i.Id == id);
                entities.Applications.Remove(item);
                entities.SaveChanges();
            }

            return Redirect("/Application");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Update(Guid id)
        {
            //check for tenantId and refresh token in session
            if (Session["TenantID"] == null || Session["RefreshToken"] == null)
                return RedirectToAction("Error", "Home", new { error = "Session expired" });
            var tenantId = Session["TenantID"].ToString();
            var refreshToken = Session["RefreshToken"].ToString();

            //use authentication context to get access token to azure graph
            AuthenticationContext context = new AuthenticationContext(string.Format("{0}/{1}", SettingsHelper.AuthorizationUri, tenantId));
            var result = await context.AcquireTokenByRefreshTokenAsync(refreshToken, new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret), SettingsHelper.AADGraphResourceId);

            //get the registered app
            using (ApplicationEntities entities = new ApplicationEntities())
            {
                var tenantIdGuid = new Guid(tenantId);
                var dbApp = entities.Applications.FirstOrDefault(i => i.TenantId == tenantIdGuid && i.Id == id);
                var app = new ApplicationModel()
                {
                    CliendId = dbApp.Id,
                    Name = dbApp.Name,
                    AppOriginsFlat = dbApp.Origins,
                };
                app.AppOrigins = app.AppOriginsFlat.Split(';').ToList();

                //get the application from Azure AD to validate settings
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + result.AccessToken);
                client.DefaultRequestHeaders.Add("Accept", "application/json; odata=verbose");
                using (HttpResponseMessage response = await client.GetAsync(new Uri(string.Format("https://graph.windows.net/{0}/applications?$filter=appId eq '{1}'&api-version=1.5", tenantId, id.ToString()), UriKind.Absolute)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        JObject oResponse = JObject.Parse(json);
                        var item = oResponse.SelectToken("d.results").ToObject<List<JsonApplication>>().FirstOrDefault();
                        app.SignOnURL = item.homepage;

                        //flatten the actual scopes
                        List<string> scopeIds = new List<string>();
                        foreach (var resource in item.requiredResourceAccess.results)
                        {
                            foreach (var scope in resource.resourceAccess.results)
                                scopeIds.Add(scope.id);
                        }

                        //update scopes based on what is selected
                        app.Permissions = PermissionModel.GetAllPermissions();
                        foreach (var perm in app.Permissions)
                        {
                            perm.Selected = scopeIds.Contains(perm.ScopeId.ToString());
                        }
                    }
                }

                return View(app);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Update(Guid id, ApplicationModel application)
        {
            //TODO
            return Redirect("/Application");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            return View(new ApplicationModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add(ApplicationModel application)
        {
            //check for tenantId and refresh token in session
            if (Session["TenantID"] == null || Session["RefreshToken"] == null)
                return RedirectToAction("Error", "Home", new { error = "Session expired" });
            var tenantId = Session["TenantID"].ToString();
            var refreshToken = Session["RefreshToken"].ToString();

            //use authentication context to get access token to azure graph
            AuthenticationContext context = new AuthenticationContext(string.Format("{0}/{1}", SettingsHelper.AuthorizationUri, tenantId));
            var result = await context.AcquireTokenByRefreshTokenAsync(refreshToken, new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret), SettingsHelper.AADGraphResourceId);

            //determine which scopes are selected
            List<Scopes> scopes = new List<Scopes>();
            foreach (var scope in AppScopes.ScopeIds.Keys)
            {
                if (Request[AppScopes.ScopeIds[scope]] != null)
                {
                    scopes.Add(scope);
                }
            }

            //get the domain
            var upn = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
            upn = upn.Substring(upn.IndexOf('@') + 1);
            upn = upn.Substring(0, upn.IndexOf('.'));

            //create the application registration
            var appResult = AppRegistration.CreateWebAppRegistration(result.AccessToken, tenantId, application.Name, Request["hdnSignOnUrlPrefix"] + application.SignOnURL,
                String.Format("https://{0}.onmicrosoft.com/{1}", upn, application.Name.Replace(" ", "")), "https://easyauth.azurewebsites.net/OAuth/AuthCode", true, true, scopes);

            //Add to database
            using (ApplicationEntities entities = new ApplicationEntities())
            {
                Application app = new Application()
                {
                    Id = new Guid(appResult["client_id"]),
                    Secret = appResult["client_secret"],
                    Origins = Request["AppOriginsFlat"],
                    Name = application.Name,
                    TenantId = new Guid(tenantId)
                };
                entities.Applications.Add(app);
                entities.SaveChanges();
            }

            return Redirect("/Application");
        }
    }
}