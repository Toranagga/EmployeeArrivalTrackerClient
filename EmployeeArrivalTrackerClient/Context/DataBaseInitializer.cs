using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<EmployeeArrivalTrackerDBContext>
    {
    }
}