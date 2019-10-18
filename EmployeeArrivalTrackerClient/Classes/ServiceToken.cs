using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    public class ServiceToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}