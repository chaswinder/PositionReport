namespace PositionReport.Scheduler
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using global::Common.Logging;

    using Topshelf;

    using PositionReport.Scheduler.Service;
    using PositionReport.Scheduler.Configuration;

    // *****************************************************************
    // To install as a service, run PositionReport.Scheduler.exe INSTALL
    // *****************************************************************

    public class Program
    {
        private const string ServiceName = "PositionReportService";
        
        public static void Main()
        {
            // Get a local ILog instance so we can log the service starting stopping and erroring
            var log = LogManager.GetLogger<Program>();

            // Hook up exception handlers
            AttachToExceptions();

            // read application configuration settings
            var config = new Config();

            // Host windows service using Topshelf
            HostFactory.Run(x =>
            {
                x.Service<ReportService>(s =>
                {
                    s.ConstructUsing(name => new ReportService(config));
                    s.WhenStarted(svc =>
                    {
                        log.Info(string.Format("{0} service started", ServiceName));
                        svc.Start();
                    });

                    s.WhenStopped(svc =>
                    {
                        log.Info(string.Format("Stopping service {0}...", ServiceName));
                        svc.Stop();
                        log.Info(string.Format("{0} has stopped", ServiceName));
                    });
                });

                x.RunAsLocalSystem();
                x.SetServiceName(ServiceName);
                x.SetDisplayName(ServiceName);
                x.SetDescription(ServiceName);
            });

        }        

        // Setup boilerplate exception handling events
        public static void AttachToExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => OnFatalException((Exception)args.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (sender, args) => OnFatalException(args.Exception);
        }

        private static void OnFatalException(Exception e)
        {
            var log = LogManager.GetLogger<Program>();
            var message = string.Format("A fatal exception has occurred in the {0} service: {1}", ServiceName, e.Message    );
            log.Error(message);
            Console.WriteLine(message);
        }
    }
}
