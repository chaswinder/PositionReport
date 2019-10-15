namespace PositionReport.Scheduler.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global::Common.Logging;

    using Quartz;
    using Quartz.Impl;

    using PositionReport.Scheduler.Configuration;

    public class ReportService
    {
        private readonly ILog log;
        private readonly IConfig config;
        private IScheduler scheduler = null;

        public ReportService(IConfig config)
        {
            this.config = config;
            this.log = LogManager.GetLogger<ReportService>();
        }

        public void Start()
        {
            this.scheduler = this.ScheduleJob(this.config.GetIntervalInMinutes());
        }

        public void Stop()
        {
            this.log.Info("Stopping Quartz scheduler");
            if (scheduler != null)
            {
                scheduler.Shutdown();
            }
            scheduler = null;
        } 

        private IScheduler ScheduleJob(int intervalInMinutes)
        {
            const string jobKey = "PositionReport";
            var groupKey = Guid.NewGuid().ToString();

            // Create a Quartz standard scheduler factory, and use that to create a Scheduler
            IScheduler scheduler = new StdSchedulerFactory().GetScheduler();
            scheduler.Start();

            // Define a new job definition (job detail)
            IJobDetail quartzJob = JobBuilder.Create<PositionReportJob>()
                .WithIdentity(jobKey, groupKey)
                .Build();

            // Trigger the job to run immediately, and then every "intervalInMinutes" minutes
            ITrigger trigger = TriggerBuilder.Create()
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInMinutes(intervalInMinutes)
                  .RepeatForever())
              .Build();

            scheduler.ScheduleJob(quartzJob, trigger);

            return scheduler;
        }
    }
}
