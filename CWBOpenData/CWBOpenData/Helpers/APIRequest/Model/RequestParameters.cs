﻿using CWBOpenData.Helpers.APIRequest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CWBOpenData.Helpers.APIRequest.Model
{
    public class RequestParameters
    {

        public RequestParameters()
        {
            SetDefaultValue();
        }


        public RequestParameters(string baseUrl, string apiUrl, HttpMethodType type)
        {
            this.BaseUrl = baseUrl;
            this.ApiUrl = apiUrl;
            this.Method = type;
            SetDefaultValue();
        }


        private void SetDefaultValue()
        {
            this.Parameters = new List<KeyValuePair<string, object>>();
            this.Headers = new List<KeyValuePair<string, string>>();
            this.Cookies = new List<KeyValuePair<string, string>>();

            this.TimeoutMillisecond = 45000;  //45秒   
            this.ContentType = "application/json";
            this.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
        }


        /// <summary>
        /// domain url 尾巴請加斜線
        /// </summary>
        public string BaseUrl { get; set; }


        public string ApiUrl { get; set; }


        public int TimeoutMillisecond { get; set; }


        public HttpMethodType Method { get; set; }


        public EncodingType EncodingType { get; set; }


        public string ContentType { get; set; }


        public string Accept { get; set; }


        public string Referer { get; set; }


        public string UserAgent { get; set; }

        public string Host { get; set; }

        public AdvancedSetting AdvancedSetting { get; set; }



        public Action<RequestResultTempModel> LogAction { get; set; }

        public List<KeyValuePair<string, object>> Parameters { get; set; }

        public List<KeyValuePair<string, string>> Headers { get; set; }

        public List<KeyValuePair<string, string>> Cookies { get; set; }

    }



    public class AdvancedSetting
    {
        public bool AllowAutoRedirect { get; set; }


        public WebProxy Proxy { get; set; }
    }




    public class TraceRedirectInfoDto
    {
        public TraceRedirectInfoDto()
        {
            this.Cookies = new Dictionary<string, string>();
        }

        public string ResponseText { get; set; }

        public string RedirectUrl { get; set; }

        public Dictionary<string, string> Cookies { get; set; }
    }
}
