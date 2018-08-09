using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class User
    {
        public string username { get; set; }
        public string role { get; set; }
        public AccessTokenModel accessToken{get; set;}
        
        //public AccessToken{get;set;}
    }
}
