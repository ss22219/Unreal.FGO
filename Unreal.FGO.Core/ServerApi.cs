using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public BaseResponse LastResponse { get; set; }
        public List<HomeUserquest> userQuest = new List<HomeUserquest>();
        public HomeUsergame userGame { get; set; }

        public HttpClient Client = new HttpClient(new HttpClientHandler()
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip
        });

        public HttpClient BSGameSDKClient = new HttpClient(new HttpClientHandler()
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.GZip
        });

        public int ApNum
        {
            get
            {
                return GetApNum(int.Parse(userGame.actMax), long.Parse(userGame.actRecoverAt));
            }
        }

        public int GetApNum(int actMax, long actRecoverAt)
        {
            long num = actRecoverAt - TimetampSecond;
            if (num > 0L)
            {
                long num2 = ((num + 300) - 1L) / ((long)300);
                return ((actMax <= num2) ? 0 : (actMax - ((int)num2)));
            }
            return actMax;
        }

        public long serverOffsetTime = 0;
        protected static DateTime dtUnixEpoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public long Timetamp
        {
            get
            {
                return ((long)DateTime.Now.AddSeconds(serverOffsetTime).Subtract(dtUnixEpoch).TotalMilliseconds); ;
            }
        }
        public long TimetampSecond
        {
            get
            {
                return ((long)DateTime.Now.AddSeconds(serverOffsetTime).Subtract(dtUnixEpoch).TotalSeconds); ;
            }
        }
        public const string iOSGameServer = "http://line1.s1.ios.fate.biligame.net";
        public const string AndroidGameServer = "https://line1.s1.bili.fate.biligame.net";
        public string UnityVersion = "5.3.3f1";
        public string ApiVersion = "60";
        public string UserAgent { get { return PlatfromInfos["user_agent"]; } set { PlatfromInfos["user_agent"] = value; } }
        public string CurrGameServer;
        public static readonly string[] saveNames = new string[] { "user_id", "access_key", "access_token", "deviceid", "usk", "model", "uname", "s_face", "face", "uid", "idfa", "udid", "cookie" };

        public void Update(Dictionary<string, string> data)
        {
            foreach (var item in data)
            {
                platfromInfos[item.Key] = item.Value;
            }
        }

        private string GetCookie(IEnumerable<string> oldValues, IEnumerable<string> values)
        {
            var reg = new Regex(@"([^;=\s]+)=([^;=\s]+)");
            string cookie = "";
            var dic = new Dictionary<string, string>();
            foreach (var item in oldValues)
            {
                var matchs = reg.Matches(item);
                if (matchs.Count > 0)
                {
                    foreach (Match match in matchs)
                    {
                        if (match.Groups[1].Value != "Path" && match.Groups[1].Value != "Expires" && match.Groups[1].Value != "Domain")
                        {
                            dic[match.Groups[1].Value] = match.Groups[2].Value;
                        }
                    }
                }
            }
            foreach (var item in values)
            {
                var matchs = reg.Matches(item);
                if (matchs.Count > 0)
                {
                    foreach (Match match in matchs)
                    {
                        if (match.Groups[1].Value != "Path" && match.Groups[1].Value != "Expires" && match.Groups[1].Value != "Domain")
                        {
                            dic[match.Groups[1].Value] = match.Groups[2].Value;
                        }
                    }
                }
            }
            foreach (var item in dic.Keys.OrderBy(K => K))
            {
                cookie += string.Format(" {0}={1};", item, dic[item]);
            }
            return cookie.TrimEnd(';').Trim();
        }

        public async Task<byte[]> GetCaptcha()
        {
            var url = "https://account.bilibili.com/captcha";
            var response = await BSGameSDKClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Headers.Contains("Set-Cookie"))
                {
                    var old = BSGameSDKClient.DefaultRequestHeaders.Contains("Cookie") ? BSGameSDKClient.DefaultRequestHeaders.GetValues("Cookie") : new string[] { };
                    var cookie = GetCookie(old, response.Headers.GetValues("Set-Cookie"));
                    BSGameSDKClient.DefaultRequestHeaders.Remove("Cookie");
                    BSGameSDKClient.DefaultRequestHeaders.Add("Cookie", cookie);
                }
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return bytes;
            }
            return null;
        }

        public Dictionary<string, string> SaveData()
        {
            var data = new Dictionary<string, string>();
            foreach (var item in PlatfromInfos.Where(d => saveNames.Contains(d.Key)))
            {
                data.Add(item.Key, item.Value);
            }
            return data;
        }

        public Dictionary<string, string> PlatfromInfos
        {
            get
            {
                return platfromInfos;
            }
        }

        private Dictionary<string, string> platfromInfos;

        public bool ios { get; set; }

        public ServerApi(bool ios = true)
        {
            this.ios = ios;
            if (ios)
                SetIos();
            else
                SetAndroid();

            Client.DefaultRequestHeaders.ExpectContinue = false;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            Client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-cn"));
            Client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            Client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            Client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            Client.DefaultRequestHeaders.Add("X-Unity-Version", UnityVersion);
            Client.DefaultRequestHeaders.Connection.Add("keep-alive");
            Client.DefaultRequestHeaders.Connection.Add("keep-alive");

            BSGameSDKClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            BSGameSDKClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-cn"));
            BSGameSDKClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            BSGameSDKClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            BSGameSDKClient.DefaultRequestHeaders.ExpectContinue = false;
            BSGameSDKClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            BSGameSDKClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            BSGameSDKClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BSGameSDK");
        }

        private async Task<T> getResult<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private static ByteArrayContent content(string text)
        {
            return new ByteArrayContent(Encoding.UTF8.GetBytes(text));
        }

        public void SetAndroid()
        {
            setAndroidPlatFromInfo();
            CurrGameServer = AndroidGameServer;
            UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 10_2 like Mac OS X) AppleWebKit/602.3.12 (KHTML, like Gecko) Mobile/14C92";
            UnityVersion = "5.3.3f1";
        }

        public void SetIos()
        {
            setIosPlatFromInfo();
            CurrGameServer = iOSGameServer;
            UserAgent = @"Mozilla/5.0 (iPhone; CPU iPhone OS 10_2 like Mac OS X) AppleWebKit/602.3.12 (KHTML, like Gecko) Mobile/14C92";
            UnityVersion = "5.3.3f1";
        }

        private void setAndroidPlatFromInfo()
        {
            platfromInfos = new Dictionary<string, string>() {
                       { "t","22360"},
                       { "deviceid","6543e0048c27a4eb3d2a74fc93b3e680"},
                       { "v","1.0.1"},
                       { "s","1"},
                       { "mac","00000000000000E0"},
                       { "os","Android OS 4.4.2 / API-19 (KOT49H/I9500XXUFNC1)"},
                       { "ptype","samsung GT-I9500"},
                       { "imei","aaaaa"},
                       { "username","admin@qq.com"},
                       { "type","token"},
                       { "password","111111"},
                       { "rksdkid","1"},
                       { "rkchannel","24"},
                       { "appVer","1.14.0"},
                       { "dateVer","53"},
                       { "try",""},
                       { "developmentAuthCode","aK8mTxBJCwZyxBjNJSKA5xCWL7zKtgZEQNiZmffXUbyQd5aLun"},
                       { "version","53"},
                       { "dataVer","53"},
                       { "server_id","248"},
                       { "original_domain",""},
                       { "model","GT-I9500"},
                       { "pf_ver","4.4.2"},
                       { "c","0"},
                       { "domain_switch_count","0"},
                       { "ver","1.14.0"},
                       { "udid","aQ06CW9XNgcyBjYGegZ6"},
                       { "net","4"},
                       { "sdk_log_type","1"},
                       { "sdk_ver","1.5.4"},
                       { "channel_id","1"},
                       { "sdk_type","1"},
                       { "platform_type","3"},
                       { "game_id","112"},
                       { "domain","pinterface.biligame.net"},
                       { "operators","1"},
                       { "ad_ext","{\"ad_f\":\"36938c908e0c11e6bf0552223301d6e6\",\"ad_location_id\":\"305\",\"ad_channel_id\":\"47\"}"},
                       { "app_id","112"},
                       { "merchant_id","1"},
                       { "dp","1280*720"},
                       { "res","0"},
                       { "code","0"},
                       { "rkuid","1"},
                       { "rgsid","1001"},
                       { "idfa",""},
                       { "assetbundleFolder",""},
                       { "userAgent","1"},
                       { "ac","action"},
                       { "key","toplogin"},
                       { "umk",""},
                       { "sgtype","2"},
                       { "sgtag","20161229"},
                       { "server_name","bili区"},
                       { "grant_pos","1"},
                       { "mid","2657998"}
                    };
        }

        private void setIosPlatFromInfo()
        {
            platfromInfos = new Dictionary<string, string>() {
               { "user_agen","fatego/1.8.30 CFNetwork/808.2.16 Darwin/16.3.0"},
               { "_userId","100101100121"},
               { "ac","home"},
               { "key","toplogin"},
               { "deviceid","32584A9A-C83F-43D6-8C70-905E5E31E18E"},
               { "os","iOS 10.2"},
               { "ptype","iPhone9,2"},
               { "umk",""},
               { "rgsid","1001"},
               { "rkchannel","996"},
               { "userId","100101100121"},
               { "appVer","1.14.0"},
               { "dateVer","1482868800"},
               { "try",""},
               { "developmentAuthCode","aK8mTxBJCwZyxBjNJSKA5xCWL7zKtgZEQNiZmffXUbyQd5aLun"},
               { "userAgent","1"},
               { "dataVer","53"},
               { "a","3581730"},
               { "presentIds","[3581730,3581731,3581728,3581729]"},
               { "t","20399"},
               { "v","1.0.1"},
               { "s","1"},
               { "mac","0"},
               { "imei",""},
               { "username","username"},
               { "type","login"},
               { "password","111111"},
               { "rksdkid","1"},
               { "version","1"},
               { "c","0"},
               { "channel_id","1000"},
               { "domain","pinterface.biligame.net"},
               { "dp","1242*2208"},
               { "game_id","125"},
               { "idfa","12533453-F1CD-4BBF-814C-E4FDD4F9F62D"},
               { "merchant_id","1"},
               { "model","iPhone9,2"},
               { "net","4"},
               { "operators","4"},
               { "pf_build","14C92"},
               { "pf_ver","10.2"},
               { "platform_type","1"},
               { "sdk_log_type","3"},
               { "sdk_type","2"},
               { "sdk_ver","1.5.3.1"},
               { "server_id","296"},
               { "timestamp","1482905907000"},
               { "udid","E30328F8-FD39-4557-88F6-A16A3FB42E46"},
               { "ver","1.14.0"},
               { "sign","bce9dee706e40c85ea3f0b763a3b0571"},
               { "app_id","125"},
               { "code","0"},
               { "platform","1"},
               { "res","0"},
               { "rkuid","1"},
               { "rguid","1100121"},
               { "rgusk","7cedd99fa5b7080ca1d4"},
               { "assetbundleFolder",""},
               { "server_name","bili区"},
               { "sgtype","2"},
               { "sgtag","20161007"}
            };
            platfromInfos["t"] = "22360";
            platfromInfos["imei"] = "aaaaa";
            platfromInfos["os"] = "iOS 10.2";
            platfromInfos["ptype"] = "iPhone9,2";
            platfromInfos["mac"] = "00000000000000E0";
            platfromInfos["user_agent"] = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_2 like Mac OS X) AppleWebKit/602.3.12 (KHTML, like Gecko) Mobile/14C92";
        }

        public Dictionary<string, string> getPlatfromInfoDic(string[] names, Dictionary<string, string> dic = null)
        {
            if (dic == null)
                dic = new Dictionary<string, string>();
            foreach (var item in names)
            {
                if (string.IsNullOrEmpty(item) || !PlatfromInfos.ContainsKey(item))
                    continue;
                var data = PlatfromInfos[item];
                dic[item] = data == null ? string.Empty : data;
            }
            return dic;
        }

        public Dictionary<string, string> getPlatfromInfoDic(string names, Dictionary<string, string> dic = null)
        {
            names = new Regex("=[^&]+").Replace(names, "");
            return getPlatfromInfoDic(names, dic);
        }

        Regex queryRegex = new Regex("([^&=?]+)=([^&=?]*)");
        public Dictionary<string, string> getQueryDic(string query, Dictionary<string, string> dic = null)
        {
            if (dic == null)
                dic = new Dictionary<string, string>();
            var matchs = queryRegex.Matches(query);
            if (matchs.Count > 0)
            {
                foreach (Match item in matchs)
                {
                    dic[item.Groups[1].Value] = System.Web.HttpUtility.UrlDecode(item.Groups[2].Value);
                }
            }
            return dic;
        }

        public string getPlatfromInfo(string names)
        {
            return DicToUrl(getPlatfromInfoDic(names));
        }

        public string DicToUrl(Dictionary<string, string> dic)
        {
            var sb = new StringBuilder();
            foreach (var item in dic)
            {
                sb.AppendFormat("{0}={1}&", item.Key, System.Web.HttpUtility.UrlEncode(item.Value));
            }
            return sb.ToString().TrimEnd('&');
        }

        public string getPlatfromInfo(params string[] names)
        {
            return DicToUrl(getPlatfromInfoDic(names));
        }
    }
}
