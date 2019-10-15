using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PositionReport.Scheduler.Configuration
{
    public interface IConfig
    {
        int GetIntervalInMinutes();
        string GetCSVLocation();
    }

    public class Config : IConfig
    {
        public int GetIntervalInMinutes()
        {
            int result = 60; //default if no config value set is to run every hour

            var setting = ConfigurationManager.AppSettings["IntervalInMinutes"];

            if (!string.IsNullOrEmpty(setting))
            {
                int value;
                if (Int32.TryParse(setting, out value))
                {
                    result = value;
                }
            }

            return result;
        }

        public string GetCSVLocation()
        {
            return ConfigurationManager.AppSettings["CSVLocation"];
        }
    }
}
