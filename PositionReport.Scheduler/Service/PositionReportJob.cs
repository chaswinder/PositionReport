using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using global::Common.Logging;

using Quartz;

using Services;

using PositionReport.Scheduler.Configuration;
using PositionReport.Data;
using PositionReport.Data.Model;

namespace PositionReport.Scheduler.Service
{
    public interface IPositionReportJob : IJob
    {
    }

    public class PositionReportJob : IPositionReportJob
    {
        private readonly ILog log;
        private readonly IConfig config;
        private readonly IPowerServiceProvider provider;

        // constructor allowing injection of dependencies
        public PositionReportJob(IConfig config, IPowerServiceProvider provider)
        {
            this.config = config;
            this.provider = provider;
            this.log = LogManager.GetLogger<ReportService>();
        }

        public PositionReportJob()
        {
            this.config = new Config();
            this.provider = new PowerServiceProvider();
            this.log = LogManager.GetLogger<ReportService>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var retries = 5;

            log.Info("Execute method called");

            if (!context.ScheduledFireTimeUtc.HasValue)
            {
                log.Info("Indeterminate ScheduledFireTimeUtc value for Quartz job, aborting...");
                return;
            }

            // Extract the rundate from the scheduled trigger time
            var runForDate = context.ScheduledFireTimeUtc.Value.Date;

            // Retry mechanism to carry out 5 attempts at running the report for one
            // scheduled triggering of the Execute method, before giving up.  The Power Service
            // sometimes throws exceptions...
            while (retries > 0)
            { 
                retries--;

                try
                {
                    log.Info(string.Format("Generating position report for date {0}...", runForDate.ToString("yyyyMMdd")));

                    // Get the aggregated positions by period for the specified run date
                    // NOTE: given that this is a DayAhead position report, should it be runForDate+1 ?
                    var periodVolumes = provider.GetData(runForDate);

                    // output the data to the CSV
                    SaveToCSV(periodVolumes);

                    retries = 0; // success, no retries needed
                }
                catch (Exception ex)
                {
                    // did the call to the provider fail?
                    log.Error(string.Format("An error occurred trying to retrieve PowerTrades: {0}", ex.Message));
                }

                if (retries > 0)
                {
                    log.Error("Attempting to retry generating the position report after a delay...");

                    Thread.Sleep(5000); // 5 seconds
                }
            }
        }

        private void SaveToCSV(IEnumerable<PeriodVolume> output)
        {
            // generate CSV output using a StringBuilder
            var sb = new StringBuilder();

            // header line
            sb.AppendLine("Period,Volume");

            foreach (var line in output)
            {
                // leading zero for the Period, but no formatting / decimalplace truncation for Volume
                sb.AppendLine(string.Format("{0:D2}:00,{1:F}", line.GetStartHour(), line.Volume));
            }

            var dateTimeNow = DateTime.Now;

            // if the output folder doesn't exit, attempt to create it
            if (!Directory.Exists(this.config.GetCSVLocation()))
            {
                Directory.CreateDirectory(this.config.GetCSVLocation());
            }

            // eg. PowerPosition_YYYYMMDD_HHMM.csv
            var filename = this.config.GetCSVLocation()+"\\"
                +string.Format("PowerPosition_{0}_{1}.csv", dateTimeNow.ToString("yyyyMMdd"), dateTimeNow.ToString("HHmm"));

            // Save csv file, asusme there won't be a file already there with the same name (file locking)
            File.WriteAllText(filename, sb.ToString());

            log.Info(string.Format("CSV report output saved to file {0}.", filename));
        }
    }
}
