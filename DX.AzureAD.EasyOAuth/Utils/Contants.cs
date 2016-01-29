using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Utils
{
    public class Constants
    {
        public const string AuthorizationUri = "https://login.microsoftonline.com/common/oauth2/authorize";
        public const string TokenUri = "https://login.microsoftonline.com/common/oauth2/token";
        public const string LogoutUri = "https://login.microsoftonline.com/common/oauth2/logout";
        public const string discoveryServiceResourceId = "https://api.office.com/discovery/";
        public const string discoveryServiceEndPoint = "https://api.office.com/discovery/v1.0/me/services";

        public const string capabilitiesTagStr = "discovery_capability";
        public const string stateTagStr = "state";
        public const string authCodeTagStr = "auth_code";
        public const string accessTokenTagStr = "access_token";
        public const string refreshTokenTagStr = "refresh_token";
        public const string azureUserTagStr = "azure_user";
        public const string azureUserEmailTagStr = "azure_email";
        public const string azureUserTenantIdTagStr = "azure_tid";

        public const string discoveryServiceTokenTagStr = "disc_access_token";
        public const string errorTagStr = "error";
        public const string errorDescrptionTagStr = "error_description";
        public const string errorMessageTagStr = "error_message";
        public const string clientIdTagStr = "client_id";
        public const string userLoggedInStr = "logged_in";
        public const string platformNameTagStr = "user_platform";
        public const string appNameTagStr = "user_app";

        public const string productNameTagStr = "product";
        public const string originPage = "originalPage";

#if DEBUG
        public const string redirectUri = "https://localhost:44302/Home/rest";
#else
        public const string redirectUri = "https://bcd5c4ca-866e-4bad-b20d-82864241870b.azurewebsites.net";
#endif
    }
}
