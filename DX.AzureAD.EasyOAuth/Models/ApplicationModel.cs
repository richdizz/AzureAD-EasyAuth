using DX.AzureAD.EasyOAuth.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Models
{
    public class ApplicationModel
    {
        public ApplicationModel()
        {
            AppOrigins = new List<string>();
            Permissions = PermissionModel.GetAllPermissions();
        }
        public string Name { get; set; }
        public Guid CliendId { get; set; }
        [Display(Name = "Sign on URL")]
        public string SignOnURL { get; set; }
        public List<string> AppOrigins { get; set; }
        public string AppOriginsFlat { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
}
