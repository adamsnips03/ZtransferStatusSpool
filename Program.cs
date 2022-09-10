using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NLog.Web;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ZTransferStatusSpool.Models;
using ZTransferStatusSpool.Services;
using Newtonsoft.Json;
using Topshelf;

namespace ZTransferStatusSpool
{
    class Program
    {
        public static bool isRunning = false;
        public static bool startSpooling = false;
        private static System.Timers.Timer intervalTime;
        public static Logger _logger;
        static void Main(string[] args)
        {
            _logger = LogManager.GetCurrentClassLogger();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args).Build();

            using var servicesProvider = new ServiceCollection()
         .AddLogging(loggingBuilder => {
             // configure Logging with NLog
             loggingBuilder.ClearProviders();
             loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
             loggingBuilder.AddNLog(config);
         }).BuildServiceProvider();

            SystemSettings settings = config.GetRequiredSection("Settings").Get<SystemSettings>();
            intervalTime = new System.Timers.Timer();

            Console.WriteLine("Starting service....");
            _logger.Log(NLog.LogLevel.Info, "Starting service....");

            try
            {
                var exitCode = HostFactory.Run(x => {
                    x.Service<ZTransferSchedular>(s =>
                    {
                        s.ConstructUsing(ztransfer => new ZTransferSchedular(settings));
                        s.WhenStarted(ztransfer => ztransfer.Start());
                        s.WhenStopped(ztransfer => ztransfer.Stop());
                    });
                    x.RunAsLocalSystem();
                    x.SetServiceName("ZTransferStatusCheck");
                    x.SetDisplayName("ZTransfer status check Schedular");
                    x.SetDescription("Get Origin Tracer number base on a particular status code then make an http call to an endpoint that will handle the rest.  ");
                });

                int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
                Environment.ExitCode = exitCodeValue;
            }
            catch (Exception error)
            {
                _logger.Log(NLog.LogLevel.Error, "Topshelf>>> " + error);
            }
            

            //intervalTime.Elapsed += delegate { DoJob(settings); };
            //intervalTime.Interval = 10000;
            //intervalTime.Enabled = true;
            //Console.WriteLine("");
            //Console.ReadKey();

        }

        //public static void DoJob(SystemSettings settings)
        //{
        //    getAllOriginTracerNo(settings);          
        //    Console.WriteLine("\n");
        //}

        //public static void getAllOriginTracerNo(SystemSettings settings)
        //{
        //    //String username = settings.ApiUserName;
        //    //String password = settings.ApiUserPassword;
        //    //String apiKey = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
           
        //    //var host = settings.Host;
        //    string apiKey = settings.ApiKey;
        //    string endpoint = settings.Endpoint;
        //    var _utility = new UtilityService();

        //     Console.WriteLine("Starting Service....");
        //    startSpooling = true;
        //    if (startSpooling && !isRunning)
        //    {
        //        isRunning = true;
        //        new Thread(() =>
        //        {
        //            Thread.CurrentThread.IsBackground = true;
        //            try
        //            {
        //                Console.WriteLine(DateTime.Now + ": Getting Origin Tracer Number started");
        //                _logger.Log(NLog.LogLevel.Info, ": Getting Origin Tracer Number started at "+ DateTime.Now );
        //                var reqResult = new RequestByStatus(settings);
                        
        //                var originRefs = reqResult.GetAllRequest();
                       
        //                foreach (var originTracer in originRefs)
        //                {
        //                    var payload = new RequestRef()
        //                    {
        //                        OriginTracerNo = originTracer.OriginTracerNo
        //                    };
        //                    string sPayload = JsonConvert.SerializeObject(payload);

        //                    var ZTResponse = _utility.PostHttpRequest<ZTransferResponse>(endpoint, sPayload, apiKey);
                            
        //                    _logger.Log(NLog.LogLevel.Info, "Origin Tracer Nnumber >>> " + originTracer.OriginTracerNo);
        //                    //_logger.Log(NLog.LogLevel.Info, "ZTransfer Response Body >>> " + JsonConvert.SerializeObject(ZTResponse));

        //                }
        //                _logger.Log(NLog.LogLevel.Info, ": Getting Origin Tracer Number stoped at " + DateTime.Now);
        //                isRunning = false;
        //            }
        //            catch (Exception exception)
        //            {
        //                var error = exception.Message;
        //                _logger.Log(NLog.LogLevel.Error, "Get reference: " + error);
        //                Console.WriteLine(DateTime.Now + ": ERROR... " + error);
        //            }
        //        }).Start();
        //    }
        //}

       

    }
}
