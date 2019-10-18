using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    /// <summary>
    /// Custom EmployeeArrivalTrackerDB contex class.
    /// </summary>
    public class EmployeeArrivalTrackerDBContext : DbContext
    {
        public EmployeeArrivalTrackerDBContext() : base("EmployeeArrivalTrackerClient")
        { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDailyArrivalTime> EmployeeDailyArrivalTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}