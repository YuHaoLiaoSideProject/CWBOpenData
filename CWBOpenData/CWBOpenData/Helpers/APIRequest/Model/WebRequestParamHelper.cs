using CWBOpenData.Extensions;
using CWBOpenData.Helpers.APIRequest.Enums;
using CWBOpenData.Helpers.APIRequest.Extension;
using System.Collections.Generic;

namespace CWBOpenData.Helpers.APIRequest.Model
{
    public class WebRequestParamHelper
    {
        RequestParameters _para = null;

        object _postBodyModel = null;
        public WebRequestParamHelper(string baseUrl, string apiUrl)
        {
            _para = new RequestParameters()
            {
                BaseUrl = baseUrl,
                ApiUrl = apiUrl
            };
        }
        public WebRequestParamHelper SetEncodingType(EncodingType encodingType)
        {
            _para.EncodingType = encodingType;
            return this;
        }
        #region "header設定"
        /// <summary>
        /// Dictionary設定header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WebRequestParamHelper Header(Dictionary<string, string> webHeaders)
        {
            foreach (var item in webHeaders)
            {
                _para.AddHeaderData(item.Key, item.Value);
            }
            return this;
        }

        /// <summary>
        /// key,value設定header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WebRequestParamHelper AddHeader(string key, string value)
        {
            _para.AddHeaderData(key, value);

            return this;
        }
        #endregion

        #region "Parameter設定"
        /// <summary>
        /// Dictionary設定參數
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WebRequestParamHelper AddParameter(string key, object value)
        {
            _para.AddParameter(key, value);

            return this;
        }

        public object GetPostBodyModel()
        {
            return _postBodyModel;
        }

        public WebRequestParamHelper SetPostBodyModel(object postBodyModel)
        {
            _postBodyModel = postBodyModel;

            return this;
        }
        /// <summary>
        /// Dictionary設定參數
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WebRequestParamHelper AddParameter(Dictionary<string, string> webParameter)
        {
            if (webParameter.IsEmptyOrNull())
                return this;

            foreach (var item in webParameter)
            {
                _para.AddParameter(item.Key, item.Value);
            }
            return this;
        }
        #endregion

        public void SetHttpMethod(HttpMethodType method)
        {
            _para.Method = method;
        }

        public RequestParameters GetParameters()
        {
            return _para;
        }

        /// <summary>
        /// 新加一項 Cookie
        /// </summary>
        public WebRequestParamHelper AddCookieValues(string key, string value)
        {
            _para.Cookies.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public WebRequestParamHelper SetParamReferer(string referer)
        {
            _para.Referer = referer;
            return this;
        }
    }
}
