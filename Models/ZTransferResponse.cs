using System;
using System.Collections.Generic;
using System.Text;

namespace ZTransferStatusSpool.Models
{
    public class ZTransferResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Sessionid { get; set; }
        public string ResponseCode { get; set; }
    }
}
