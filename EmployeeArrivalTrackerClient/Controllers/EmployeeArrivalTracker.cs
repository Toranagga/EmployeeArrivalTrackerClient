using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeArrivalTrackerClient
{
    public class EmployeeArrivalTracker : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchByDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchByDate(DateTime date)
        {
            return View();
        }
    }
}