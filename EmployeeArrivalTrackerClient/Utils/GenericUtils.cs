using log4net;
using System;
using System.Configuration;
using System.Text;
using System.Web;

namespace EmployeeArrivalTrackerClient
{
    public class GenericUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string BuildIntialGetUrl(string date)
        {
            var callbackUrl = GetCallbackUrl();

            if (string.IsNullOrEmpty(callbackUrl))
                return null;

            StringBuilder builder = new StringBuilder();
            builder.Append(ConfigurationManager.AppSettings["Service_Subscribe_url"]);
            builder.Append(date);
            builder.Append("&callback=");
            builder.Append(callbackUrl);

            return builder.ToString();//string.Format("{0}{1}&callback={2}", ConfigurationManager.AppSettings["Service_Subscribe_url"], date, GetCallbackUrl()); // ConfigurationManager.AppSettings["Service_Subscribe_url"] + date + "&callback=" + GetCallbackUrl();    //http://localhost:54353/Home/DataReceiver";
        }

        public static string GetCallbackUrl()
        {
            try
            {
                // This code is tested to work on all environments
                var oRequest = System.Web.HttpContext.Current.Request;
                return oRequest.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/Home/DataReceiver");
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error occurrs when try to create a callback Url. Message: {0}", ex));
                return null;
            }
        }
    }
}