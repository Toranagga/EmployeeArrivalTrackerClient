using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    /// <summary>
    /// Model class which presents EmployeeArrival data.
    /// </summary>
    public class EmployeeArrivalData
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? ManagerId { get; set; }
        public string Teams { get; set; }
        public string ArrivalTime { get; set; }
    }

    /// <summary>
    /// ViewModel class.
    /// </summary>
    public class EmployeeArrivalViewModel
    {
        public List<EmployeeArrivalData> Items { get; set; }
    }
}