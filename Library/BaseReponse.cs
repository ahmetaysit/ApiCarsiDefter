using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class BaseResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
    }
}
