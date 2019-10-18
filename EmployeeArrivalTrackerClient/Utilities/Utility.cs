using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;

namespace EmployeeArrivalTrackerClient
{
    public class Utility
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static readonly string X_Fourth_Token = "X-Fourth-Token";
        public static readonly string Fourth_Monitor = "Fourth-Monitor";

        public static bool CreateServiceGetRequest(string date, ref string token)
        {
            try
            {
                var endpoint = GenericUtils.BuildIntialGetUrl(date);

                if (string.IsNullOrEmpty(endpoint))
                {
                    Log.Error(string.Format("Error in building an Url for initial Get service request for input date = '{0}'", date));
                    return false;
                }

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Accept-Client", Fourth_Monitor);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd().ToString();
                    var serviceTokenObj = JsonConvert.DeserializeObject<ServiceToken>(response);

                    if (serviceTokenObj.Expires > DateTime.Now)
                    {
                        token = serviceTokenObj.Token;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Connection service error was occurred! Message: {0}", ex));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Inserts Employee data to database only once.
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance.</param>
        /// <returns></returns>
        public static bool InsertEmployeeData(IUnitOfWork unitOfWork)
        {
            string savepath = string.Empty;
            try
            {
                var empRow = unitOfWork.EmployeeRepository.GetFirstOrDefault();

                if (empRow == null)
                {
                    savepath = AppDomain.CurrentDomain.BaseDirectory + "/Files/employees.json";
                    var _employees = JsonConvert.DeserializeObject<IList<JsonEmployee>>(new StreamReader(savepath).ReadToEnd()).ToList();

                    foreach (var row in _employees)
                    {
                        var employee = new Employee()
                        {
                            Id = row.Id,
                            Name = row.Name,
                            SurName = row.SurName,
                            Age = row.Age,
                            Email = row.Email,
                            ManagerId = row.ManagerId,
                            Teams = string.Join(",", row.Teams),
                            Role = row.Role
                        };

                        unitOfWork.EmployeeRepository.Insert(employee);
                    }
                    unitOfWork.Save();
                }
            }
            catch (SqlException ex)
            {
                Log.Error(string.Format("SqlException occurrs when try to access table dbo.Employees. Message: {0}", ex));
                return false;
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(string.Format("Error occurrs when try to get and accesss content of file {0} in db table dbo.Employees. Message: {1}", ex));
                return false;
            }
            return true;
        }

        // <summary>
        /// Inserts data comming from external service in database.
        /// </summary>
        /// <param name="data">EmployeeDailyArrivalTime array.</param>
        /// <param name="unitOfWork">UnitOfWork instance.</param>
        /// <returns></returns>
        public static bool InsertEmployeeArrivalData(EmployeeDailyArrivalTime[] data, IUnitOfWork unitOfWork)
        {
            try
            {
                foreach (var item in data)
                {
                    var isExist = unitOfWork.EmployeeDailyArrivalTimeRepository.Get(x => x.EmployeeId == item.EmployeeId && x.When.Equals(item.When));
                    if (!isExist.Any())
                    {
                        var arrivalEntity = new EmployeeDailyArrivalTime()
                        {
                            EmployeeId = item.EmployeeId,
                            When = item.When
                        };

                        unitOfWork.EmployeeDailyArrivalTimeRepository.Insert(arrivalEntity);
                        unitOfWork.Save();
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                Log.Error(string.Format("SqlException occurrs when try to access or insert in table dbo.EmployeeDailyArrivalTimes. Message: {0}", ex));
                return false;
            }
        }

        /// <summary>
        /// Builds a BuildEmployeeArrivalViewModel instance.
        /// </summary>
        /// <param name="data">EmployeeDailyArrivalTime array.</param>
        /// <param name="unitOfWork">UnitOfWork instance.</param>
        /// <returns></returns>
        public static EmployeeArrivalViewModel BuildEmployeeArrivalViewModel(EmployeeDailyArrivalTime[] data, IUnitOfWork unitOfWork)
        {
            EmployeeArrivalViewModel model = new EmployeeArrivalViewModel() { Items = new List<EmployeeArrivalData>() };

            foreach (var item in data)
            {
                var employee = unitOfWork.EmployeeRepository.GetFirstOrDefault(x => x.Id == item.EmployeeId);

                if (employee != null)
                {
                    model.Items.Add(new EmployeeArrivalData()
                    {
                        Name = employee.Name,
                        SurName = employee.SurName,
                        ManagerId = employee.ManagerId,
                        Role = employee.Role,
                        Teams = employee.Teams,
                        Email = employee.Email,
                        ArrivalTime = item.When
                    });
                }
            }

            return model;
        }
    }
}