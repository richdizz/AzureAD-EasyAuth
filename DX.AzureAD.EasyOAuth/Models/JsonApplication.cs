using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Models
{
    public class JsonApplication
    {
        public string displayName { get; set; }
        public string homepage { get; set; }
        public JsonAppIdentifiers identifierUris { get; set; }
        public bool oauth2AllowImplicitFlow { get; set; }
        public bool availableToOtherTenants { get; set; }
        public JsonAppPermissions requiredResourceAccess { get; set; }
    }

    public class JsonAppIdentifiers
    {
        public string[] results { get; set; }
    }

    public class JsonAppPermissions
    {
        public List<JsonAppResource> results { get; set; }
    }

    public class JsonAppResource
    {
        public string resourceAppId { get; set; }
        public JsonAppScopes resourceAccess { get; set; }
    }

    public class JsonAppScopes
    {
        public List<JsonAppScope> results { get; set; }
    }

    public class JsonAppScope
    {
        public string id { get; set; }
        public string type { get; set; }
    }
}
