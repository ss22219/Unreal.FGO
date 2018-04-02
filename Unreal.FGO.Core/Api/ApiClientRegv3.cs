using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using Unreal.FGO.Common;
using System.Linq;
using System.Net;
using System;
using Newtonsoft.Json;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientRegv3Res : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
    }

    public class ApiClientRegv3Req : BaseRequest<ApiClientRegv3Res>
    {
        public ApiClientRegv3Req(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override async Task<ApiClientRegv3Res> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>()
            {
                { "pwd", AesHelper.Encrypt(args[1])},
                { "sdk_type", "2"},
                { "sdk_log_type", "3"},
                { "version", "1" },
                { "domain","p.biligame.com"},
                { "account", args[0]},
                { "actionname", "reg"},
                { "type", "3"},
                { "res", "1"}
            };
            PlatfromInfos["user_id"] = args[0];
            if (args[2] != null)
                PlatfromInfos["captcha"] = args[2];
            var url = "http://p.biligame.com/api/client/regV3";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "c", "captcha", "channel_id", "domain", "domain_switch_count", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "original_domain", "pf_build", "pf_ver", "platform_type", "pwd", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "timestamp", "udid", "user_id", "ver", "version", "sign" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            var response = await Get(url, false, false, false);
            if (response.code != 0)
            {
                replaceInfos["code"] = response.code.ToString();
                url = "https://gameinfoc.biligame.net/";
                var questDic = serverApi.getQueryDic("account=gool19&actionname=reg&app_id=125&channel_id=1000&code=-105&dp=1242%2A2208&idfa=12533453-F1CD-4BBF-814C-E4FDD4F9F62D&merchant_id=1&model=iPhone9%2C2&net=4&operators=4&pf_build=14C92&pf_ver=10.2&platform=1&res=1&sdk_log_type=3&sdk_ver=1.5.3.1&server_id=296&timestamp=1483950471000&type=3&udid=E30328F8-FD39-4557-88F6-A16A3FB42E46&ver=1.14.0");
                getParam = getPlatfromInfo(questDic.Keys.ToArray());
                if (!string.IsNullOrEmpty(getParam))
                    url += "?" + getParam;
                try
                {
                    //var client = new WebClient();
                    //client.Headers["User-Agent"] = "Mozilla/5.0 BSGameSDK";
                    //client.Headers["Cookie"] = httpClient.DefaultRequestHeaders.GetValues("Cookie").First();
                    //var body =  client.DownloadString(new Uri(url));
                    //return JsonConvert.DeserializeObject<ApiClientRegv3Res>(body);
                    await Get(url, false, false, false);
                }
                catch (System.Exception)
                {
                }
            }
            return response;

        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<ApiClientRegv3Res> ApiClientRegv3(string userId, string password, string captcha = null)
        {
            return await new ApiClientRegv3Req(this).Send(userId, password, captcha);
        }
    }
}