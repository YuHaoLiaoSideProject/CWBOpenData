﻿using CWBOpenData.Helpers.APIRequest.Model;
using System.Collections.Generic;

namespace CWBOpenData.Helpers.APIRequest.Extension
{
    public static class ExtensionOfParameters
    {

        /// <summary>
        /// 擴充方法 新加一筆 request body參數
        /// </summary>
        public static void AddParameter(this RequestParameters para, string key, object value)
        {
            para.Parameters.Add(new KeyValuePair<string, object>(key, value));
        }


        /// <summary>
        /// 擴充方法  新加一項 Header參數
        /// </summary>
        public static void AddHeaderData(this RequestParameters para, string key, string value)
        {
            para.Headers.Add(new KeyValuePair<string, string>(key, value));
        }



        /// <summary>
        /// 擴充方法 新加一項 Cookie
        /// </summary>
        public static void AddCookieValues(this RequestParameters para, string key, string value)
        {
            para.Cookies.Add(new KeyValuePair<string, string>(key, value));
        }




        /// <summary>
        /// 擴充方法 新加一項 Cookie
        /// </summary>
        public static void AddCookieValues(this RequestParameters para, Dictionary<string, string> cookies)
        {
            if (cookies == null)
                return;

            foreach (var item in cookies)
            {
                para.AddCookieValues(item.Key, item.Value);
            }
        }

    }
}
