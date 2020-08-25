using CWBOpenData.Helpers.APIRequest.Enums;
using CWBOpenData.Helpers.APIRequest.Model;
using System;

namespace CWBOpenData.Helpers.APIRequest
{
    public class WebRequestHelper
    {
        WebRequestParamHelper helper = null;

        public WebRequestHelper(string baseUrl, string apiUrl)
        {
            helper = new WebRequestParamHelper(baseUrl, apiUrl);
        }
        /// <summary>
        /// Static 設定Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static WebRequestHelper Request(string baseUrl, string apiUrl)
        {
            WebRequestHelper helper = new WebRequestHelper(baseUrl, apiUrl);
            return helper;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WebRequestHelper Get(Action<WebRequestParamHelper> action = null)
        {
            SetHttpRequest(HttpMethodType.HttpGet, action);
            return this;
        }
        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WebRequestHelper Post(Action<WebRequestParamHelper> action = null)
        {
            SetHttpRequest(HttpMethodType.HttpPost, action);
            return this;
        }
        private void SetHttpRequest(HttpMethodType method, Action<WebRequestParamHelper> action = null)
        {
            helper.SetHttpMethod(method);
            if (action != null)
            {
                action(helper);
            }
        }

        public T Response<T>()
        {
            return APIRequestHelper.RequestApi<T>(helper.GetParameters(), helper.GetPostBodyModel());
        }

    }
}
