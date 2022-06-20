using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Connect360.Models
{
    public class get_roster_responsemodel
    {
        public string jid { get; set; }
        public string nick { get; set; }
        public string subscription { get; set; }
        public string ask { get; set; }
        public string group { get; set; }
    }
}