using Newtonsoft.Json;
using NLog;
using System;
using System.Timers;
using ZTransferStatusSpool.Models;

namespace ZTransferStatusSpool.Services
{
    public class ZTransferSchedular
    {
        private readonly Timer _timer;
        SystemSettings _settings;
        private Logger _logger;
        public ZTransferSchedular(SystemSettings settings)
        {
            _settings = settings;
            _logger = LogManager.GetCurrentClassLogger();
            _timer = new Timer(_settings.TimeInterval) { AutoReset = true };
            _timer.Elapsed += delegate { DoJob(_settings); };
        }

        public void DoJob(SystemSettings settings)
        {
            getAllOriginTracerNo(settings);
            Console.WriteLine("\n");
        }

        public void getAllOriginTracerNo(SystemSettings settings)
        {
            //String username = settings.ApiUserName;
            //String password = settings.ApiUserPassword;
            //String apiKey = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

            //var host = settings.Host;
            string apiKey = settings.ApiKey;
            string endpoint = settings.Endpoint;
            var _utility = new UtilityService();

            Console.WriteLine("Starting Service....");

            try
            {
                Console.WriteLine(DateTime.Now + ": Getting Origin Tracer Number started");
                _logger.Log(NLog.LogLevel.Info, ": Getting Origin Tracer Number started at " + DateTime.Now);
                var reqResult = new RequestByStatus(settings);

                var originRefs = reqResult.GetAllRequest();
                if(originRefs.Count > 0)
                {
                    foreach (var originTracer in originRefs)
                    {
                        var payload = new RequestRef()
                        {
                            OriginTracerNo = originTracer.OriginTracerNo
                        };
                        string sPayload = JsonConvert.SerializeObject(payload);
                        _logger.Log(NLog.LogLevel.Info, "Origin Tracer Nnumber >>> " + originTracer.OriginTracerNo);

                        try
                        {
                            _utility.PostHttpRequest<ZTransferResponse>(endpoint, sPayload, apiKey);
                        }
                        catch (Exception error)
                        {
                            _logger.Log(NLog.LogLevel.Error, "ZTransfer Exception Response >>> " + error);
                        }
                    }
                }
                else
                {
                    _logger.Log(NLog.LogLevel.Info, ": No Transaction found as at >>> " + DateTime.Now);
                   
                }
                _logger.Log(NLog.LogLevel.Info, ": Getting Origin Tracer Number stoped at " + DateTime.Now);
            }
            catch (Exception exception)
            {
                var error = exception.Message;
                _logger.Log(NLog.LogLevel.Error, "Get reference: " + error);
                Console.WriteLine(DateTime.Now + ": ERROR... " + error);
            }
        }


        public void Start() {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
