using CWBOpenData.Helpers.APIRequest.Enums;
using CWBOpenData.Helpers.APIRequest.Model;
using System;

namespace CWBOpenData.Helpers.APIRequest
{
    public class WebRequestHelper
    {
        WebRequestParamHelper _Param = null;

        public WebRequestHelper(string baseUrl, string apiUrl)
        {
            _Param = new WebRequestParamHelper(baseUrl, apiUrl);
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
        public WebRequestResponseHelper Get(Action<WebRequestParamHelper> action)
        {
            SetHttpRequest(HttpMethodType.HttpGet, action);
            return new WebRequestResponseHelper(_Param);
        }
        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WebRequestResponseHelper Post(Action<WebRequestParamHelper> action = null)
        {
            SetHttpRequest(HttpMethodType.HttpPost, action);
            return new WebRequestResponseHelper(_Param);
        }
        private void SetHttpRequest(HttpMethodType method, Action<WebRequestParamHelper> action = null)
        {
            _Param.SetHttpMethod(method);
            if (action != null)
            {
                action(_Param);
            }
        }
    }

    public class WebRequestResponseHelper
    {
        WebRequestParamHelper _helper = null;
        public WebRequestResponseHelper(WebRequestParamHelper helper)
        {
            _helper = helper;
        }
        public T Response<T>()
        {
            return APIRequestHelper.RequestApi<T>(_helper.GetParameters(), _helper.GetPostBodyModel());
        }
    }
}
