using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Net;

namespace ZTransferStatusSpool.Services
{
    public class UtilityService
    {
        private Logger _logger;
        public UtilityService()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public T PostHttpRequest<T>(string url, string payload, string basicAuth)
        {
           ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
           ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                var _request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                
                //_request.Proxy = new WebProxy(new Uri(_sysSettings.ProxyServer));
                _request.ContentType = "application/json";                
                _request.Method = "POST";
                
                _request.Headers.Add("Authorization", "Basic " + basicAuth);

               
                using (var _writer = new StreamWriter(_request.GetRequestStream()))
                {
                    _writer.Write(payload);
                    _writer.Flush();
                    _writer.Close();
                }
                
                var _response = _request.GetResponse() as HttpWebResponse;
                if (_response.StatusCode == HttpStatusCode.OK)
                {
                    using (var _reader = new StreamReader(_response.GetResponseStream()))
                    {
                        string sResponse = _reader.ReadToEnd();

                        var result = JsonConvert.DeserializeObject<T>(sResponse);

                        _logger.Log(NLog.LogLevel.Info, "ZTransfer Response Body >>> " + JsonConvert.SerializeObject(result));
                        //_logger.Error(" ZTransfer Response Body >>> " + JsonConvert.SerializeObject(result));

                        return result;
                    }
                }
                else
                {
                    _logger.Log(NLog.LogLevel.Error, "ZTransfer Response Body >>> " + _response);
                    //_logger.Error(" ZTransfer Response Body >>> " + _response);
                }
            }
            catch (Exception exp)
            {
                _logger.Log(NLog.LogLevel.Error, "ZTransfer Exception Response >>> " + exp.Message);
                //_logger.Error(exp.Message);
            }
            return default(T);
        }
    }
}
