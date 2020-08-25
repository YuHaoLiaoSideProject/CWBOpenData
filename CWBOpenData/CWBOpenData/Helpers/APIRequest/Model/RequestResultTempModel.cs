﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CWBOpenData.Helpers.APIRequest.Model
{
    public class RequestResultTempModel
    {

        public RequestResultTempModel()
        {
            this.ResponseCookies = new Dictionary<string, string>();
        }


        public RequestParameters RequestParameters { get; set; }

        public HttpWebRequest HttpWebRequest { get; set; }

        public string RequestContent { get; set; }

        public string ResponseContent { get; set; }

        public Dictionary<string, string> ResponseCookies { get; set; }

        public int ResponseStatusCode { get; set; }

        public long ResponseTime { get; set; }

        public System.Diagnostics.Stopwatch Timer { get; set; }




    }
}
