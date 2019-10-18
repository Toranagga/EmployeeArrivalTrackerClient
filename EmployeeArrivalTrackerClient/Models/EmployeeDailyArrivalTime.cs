using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    /// <summary>
    /// Model class.
    /// </summary>
    public class EmployeeDailyArrivalTime
    {
        [Key]
        public int ID { get; set; }
        public string When { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}