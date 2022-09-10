using System;
using System.Collections.Generic;
using System.Text;

namespace ZTransferStatusSpool.Models
{
    public class SystemSettings
    {
        public int TimeDiff { get; set; }
        public int TimeInterval { get; set; }
        public string TransStatus { get; set; }
        public string ApiUserName { get; set; }
        public string ApiUserPassword { get; set; }
        public string ApiKey { get; set; }
        public string Host { get; set; }
        public string Endpoint { get; set; }
        public string EthixConnection { get; set; }
        public string Sybase { get; set; }

    }
}
