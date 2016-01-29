﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX.AzureAD.EasyOAuth.Models
{
    public class TokenModel
    {
        public string resource { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }
        public string id_token { get; set; }

        //token_type, expires_in, expires_on, not_before, scope, pwd_exp, pwd_url
    }
}
