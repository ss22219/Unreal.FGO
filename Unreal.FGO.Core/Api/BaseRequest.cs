using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Core.Api
{
    public abstract class BaseRequest<T> where T : BaseResponse, new()
    {
        public Dictionary<string, string> PlatfromInfos { get { return serverApi.PlatfromInfos; } }
        protected static DateTime dtUnixEpoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public long Timetamp
        {
            get
            {
                return ((long)DateTime.Now.AddSeconds(serverApi.serverOffsetTime).Subtract(dtUnixEpoch).TotalMilliseconds); ;
            }
        }
        public long TimetampSecond
        {
            get
            {
                return ((long)DateTime.Now.AddSeconds(serverApi.serverOffsetTime).Subtract(dtUnixEpoch).TotalSeconds); ;
            }
        }

        public BaseRequest(ServerApi api)
        {
            this.serverApi = api;
            UserAgent = api.UserAgent;
            UnityVersion = api.UnityVersion;
            httpClient = serverApi.Client;
        }
        public abstract Task<T> Send(params string[] args);
        protected ServerApi serverApi;
        public Encoding Encoding = Encoding.UTF8;
        public string UserAgent { get; set; }
        public string UnityVersion { get; set; }

        public HttpClient httpClient;
        protected Dictionary<string, string> replaceInfos;

        public async Task<T> Get(string url, bool useGameServer = true, bool decode = true, bool addUnityHeader = true)
        {
            if (addUnityHeader)
            {
                httpClient.DefaultRequestHeaders.Remove("Cookie");
                if (serverApi.PlatfromInfos.ContainsKey("cookie"))
                    httpClient.DefaultRequestHeaders.Add("Cookie", serverApi.PlatfromInfos["cookie"]);
            }
            url = useGameServer ? serverApi.CurrGameServer + url : url;
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.Contains("Set-Cookie"))
                    serverApi.PlatfromInfos["cookie"] = response.Headers.GetValues("Set-Cookie").First().Split(';').First();

                var content = await response.Content.ReadAsStringAsync();
                if (decode)
                    content = Encoding.UTF8.GetString(Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(content)));
                var result = JsonConvert.DeserializeObject<T>(content);
                serverApi.LastResponse = result;
                result.RequestMessage = RequestMessage("Get", url, response.StatusCode);
                return result;
            }
            else
            {
                return (T)NetError(RequestMessage("Get", url, response.StatusCode));
            }
        }

        public async Task<T> Post(string url, Dictionary<string, string> parma, bool useGameServer = true, bool decode = true, bool addUnityHeader = true)
        {
            var httpContent = new FormUrlEncodedContent(parma == null ? new Dictionary<string, string>() : parma);
            if (addUnityHeader)
            {
                httpClient.DefaultRequestHeaders.Remove("Cookie");
                if (serverApi.PlatfromInfos.ContainsKey("cookie"))
                    httpClient.DefaultRequestHeaders.Add("Cookie", serverApi.PlatfromInfos["cookie"]);
            }
            url = useGameServer ? serverApi.CurrGameServer + url : url;
            var pstr = await httpContent.ReadAsStringAsync();
            string content;
            HttpResponseMessage response;
            int errorCount = 0;
        request:
            try
            {
                response = await httpClient.PostAsync(url, httpContent);
            }
            catch (Exception ex)
            {
                errorCount++;
                if (errorCount < 5)
                    goto request;
                return (T)NetError(ex.Message + "\r\n" + RequestMessage("Post", url, HttpStatusCode.GatewayTimeout, parma));
            }
            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.Contains("Set-Cookie"))
                    serverApi.PlatfromInfos["cookie"] = response.Headers.GetValues("Set-Cookie").First().Split(';').First();
                content = await response.Content.ReadAsStringAsync();
                if (decode)
                    content = Encoding.UTF8.GetString(Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(content)));
                var result = JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                serverApi.LastResponse = result;
                result.RequestMessage = RequestMessage("Post", url + "\r\n" + pstr, response.StatusCode, parma);
                return result;
            }
            else
            {
                errorCount++;
                if (errorCount < 5)
                    goto request;
                return (T)NetError(RequestMessage("Post", url, response.StatusCode, parma));
            }
        }

        public string RequestMessage(string method, string url, HttpStatusCode statusCode, Dictionary<string, string> param = null)
        {
            if (method == "Post")
                return "Post:" + url + "\r\nContent:\r\n" + JsonConvert.SerializeObject(param, Formatting.Indented) + "\r\nCode:(" + (int)statusCode + ") " + statusCode.ToString();
            else
                return "Get:" + url + "\r\nCode:(" + (int)statusCode + ") " + statusCode.ToString();
        }

        public T NetError(string message)
        {
            return new T()
            {
                code = 99,
                message = message
            };
        }
        public Dictionary<string, string> getPlatfromInfoDic(string[] names, Dictionary<string, string> replaceInfos = null)
        {
            if (names.Contains("lastAccessTime"))
                PlatfromInfos["lastAccessTime"] = TimetampSecond.ToString();
            if (names.Contains("timestamp"))
                PlatfromInfos["timestamp"] = Timetamp.ToString();
            var dic = serverApi.getPlatfromInfoDic(names);
            if (replaceInfos == null && this.replaceInfos != null)
                replaceInfos = this.replaceInfos;
            if (replaceInfos != null)
                foreach (var item in replaceInfos)
                {
                    if (!names.Contains(item.Key))
                        continue;
                    dic[item.Key] = item.Value;
                }
            if (names.Contains("sign"))
                dic["sign"] = GetSign(dic);
            if (names.Contains("usk"))
                dic["usk"] = CryptData.EncryptMD5((!serverApi.ios ? "B6949765EC73CF001718B5FD507FCD9E" : "088416A9FC66304405B483FFB1355A14") + dic["usk"]);
            var ndir = new Dictionary<string, string>();
            foreach (var name in names)
            {
                if (dic.ContainsKey(name))
                    ndir[name] = dic[name];
                else
                    ndir[name] = string.Empty;
            }
            return ndir;
        }

        public string getPlatfromInfo(string[] names, Dictionary<string, string> replaceInfos = null)
        {
            return serverApi.DicToUrl(getPlatfromInfoDic(names, replaceInfos));
        }

        public string GetSign(Dictionary<string, string> dic)
        {
            return CryptData.Sign(dic, serverApi.ios);
        }
    }
}
