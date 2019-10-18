using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeArrivalTrackerClient
{
    public class HomeController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IUnitOfWork _unitOfWork;
        private EmployeeArrivalTrackerDBContext _dbContext = new EmployeeArrivalTrackerDBContext();
        private static readonly string Model_Data_Dictionary_Key = "Model_Data_Key";
        private static readonly string X_Fourth_Token = "X-Fourth-Token";
        public static readonly AutoResetEvent ResetEvent = new AutoResetEvent(false);

        private static string Token = null;

        public static Dictionary<string, EmployeeArrivalViewModel> viewModelDictionary = new Dictionary<string, EmployeeArrivalViewModel>();

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork instance.</param>
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Non-parameterized constructor which initialze a new _unitOfWork instance.
        /// </summary>
        public HomeController()
        {
            _unitOfWork = new UnitOfWork(_dbContext);
        }

        /// <summary>
        /// Index action.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets employee arrival list by date.
        /// </summary>
        /// <param name="date">Date string.</param>
        /// <returns></returns>
        public ActionResult GetEmployeeList(string date)
        {
            if(string.IsNullOrEmpty(date))
            {
                return View("Error");
            }

            try
            {
                var existingDataRows = _unitOfWork.EmployeeDailyArrivalTimeRepository.Get(i => i.When.StartsWith(date)).ToArray();

                //If rows with existing input date exist - get them from database, otherwise make a call to the service.
                if (existingDataRows.Any())
                {
                    var model = Utility.BuildEmployeeArrivalViewModel(existingDataRows, _unitOfWork);

                    return View("EmployeeArrival", model);
                }
                else
                {
                    if (Utility.CreateServiceGetRequest(date, ref Token))
                    {
                        Utility.InsertEmployeeData(_unitOfWork);

                        ResetEvent.WaitOne();
                       
                        if (viewModelDictionary.ContainsKey(Model_Data_Dictionary_Key))
                        {
                            return View("EmployeeArrival", viewModelDictionary[Model_Data_Dictionary_Key]);
                        }
                    }

                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                var message = ex;
            }

            return View("Error");
        }

        /// <summary>
        /// Callback which is used for receiving of data from external service.
        /// </summary>
        /// <param name="data">Comming data from external service as EmployeeDailyArrivalTime array.</param>
        public void DataReceiver(EmployeeDailyArrivalTime[] data)
        {
            if(data != null && data.Any())
            {
                EmployeeArrivalViewModel model = new EmployeeArrivalViewModel();

                var headerToken = Request.Headers.Get(X_Fourth_Token);

                if (!string.IsNullOrEmpty(headerToken) && !string.IsNullOrEmpty(Token) && Token.Equals(headerToken))
                {
                    if (Utility.InsertEmployeeArrivalData(data, _unitOfWork))
                    {
                        model = Utility.BuildEmployeeArrivalViewModel(data, _unitOfWork);

                        if(!viewModelDictionary.ContainsKey(Model_Data_Dictionary_Key))
                        {
                            viewModelDictionary.Add(Model_Data_Dictionary_Key, model);
                            ResetEvent.Set();
                        }
                    }
                    else
                    {
                        Log.Warn(string.Format("EmployeeDailyArrivalTime records was not inserted in database for date: '{0}'", data[0].When));
                    }
                }
                else
                {
                    Log.Error(string.Format("Expected service response token '{0}' in callback post request missing or not valid for request with parameter 'date' = '{1}'", headerToken, data[0].When));
                }
            }
            else
            {
                Log.Error("Service not sent any data to callback method 'DataReceiver6'.");
            }
        }
    }
}