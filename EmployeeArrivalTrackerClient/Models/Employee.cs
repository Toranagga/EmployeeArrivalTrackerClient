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
    public class Employee
    {
        [KeyAttribute()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Nullable<int> ManagerId { get; set; }
        public int Age { get; set; }
        public string Teams { get; set; }
        
        public ICollection<EmployeeDailyArrivalTime> ArrivalTimes { get; set; }
    }
}