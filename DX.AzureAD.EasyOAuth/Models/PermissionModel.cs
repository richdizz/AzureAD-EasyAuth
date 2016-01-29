using DX.AzureAD.EasyOAuth.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Models
{
    public class PermissionModel
    {
        public static List<PermissionModel> GetAllPermissions()
        {
            List<PermissionModel> list = new List<PermissionModel>();

            foreach (var scope in AppScopes.ScopeIds.Keys)
            {
                list.Add(new PermissionModel() {
                    ScopeId = new Guid(AppScopes.ScopeIds[scope]),
                    ScopeCode = scope.ToString(),
                    DisplayName = AppScopes.ScopeNames[scope]
                });
            }

            return list;
        }
        public Guid ScopeId { get; set; }
        public string ScopeCode { get; set; }
        public string DisplayName { get; set; }
        public bool Selected { get; set; }
    }
}
