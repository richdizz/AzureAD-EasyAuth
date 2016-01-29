using DX.AzureAD.EasyOAuth.Models;
using DX.AzureAD.EasyOAuth.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DX.AzureAD.EasyOAuth.Controllers
{
    public class OAuthController : Controller
    {
        [Route("OAuth/AuthCode/{id}/{socket}/")]
        public async Task<ActionResult> AuthCode(string id, string socket)
        {
            //Request should have a code from AAD and an id that represents the user in the data store
            if (Request["code"] == null)
                return RedirectToAction("Error", "Home", new { error = "Authorization code not passed from the authentication flow" });
            if (Request["state"] == null || Request["state"].Split('|').Length != 2)
                return RedirectToAction("Error", "Home", new { error = "Invalid state passed to authentication flow" });
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Error", "Home", new { error = "Client id not passed from authentication flow" });
            if (String.IsNullOrEmpty(socket))
                return RedirectToAction("Error", "Home", new { error = "Socket details not passed from authentication flow" });

            //break the state into parts
            var parts = Request["state"].Split('|');

            //validate origin
            using (ApplicationEntities entities = new ApplicationEntities())
            {
                var guidId = new Guid(id);
                var item = entities.Applications.FirstOrDefault(i => i.Id == guidId);

                //TODO: validate the origin

                //get access token using the authorization code
                var token = await TokenHelper.GetAccessTokenWithCode(Request["code"], parts[1], item.Id.ToString(), item.Secret, socket);
                ViewData["token"] = JsonConvert.SerializeObject(token);
                ViewData["host"] = parts[0];

                //Send token over a socket
                TokenController.SendTokenToClient(socket, token.access_token);
            }

            //pass the refresh token to the 
            return View();
        }
    }
}