using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrossDomainAccessWebAPI.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string RoleName { get; set; }
        public string NickName { get; set; }
    }
}