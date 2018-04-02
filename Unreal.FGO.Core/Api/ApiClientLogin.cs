using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientLoginRes : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string access_key { get; set; }
        public string uid { get; set; }
        public long expires { get; set; }
    }

    public class ApiClientLoginReq : BaseRequest<ApiClientLoginRes>
    {
        public ApiClientLoginReq(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override async Task<ApiClientLoginRes> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>() {
                    { "version", "1" }
            };
            replaceInfos["user_id"] = args[0];
            replaceInfos["pwd"] = CryptData.Rsa(args[2], args[3] + args[1]);
            if (args.Length == 5)
                replaceInfos["access_key"] = args[4];
            base.replaceInfos = replaceInfos;

            var url = "https://pinterface.biligame.net/api/client/login";
            string getParam = null;
            Dictionary<string, string> postParam = null;

            if (!serverApi.ios)
                getParam = getPlatfromInfo(new string[] { "uid", "model", "pf_ver", "domain_switch_count", "ver", "net", "sdk_ver", "version", "timestamp", "game_id", "domain", "user_id", "operators", "ad_ext", "merchant_id", "dp", "server_id", "original_domain", "c", "udid", "sdk_log_type", "channel_id", "sign", "pwd", "sdk_type", "platform_type", "app_id" });
            else
                postParam = getPlatfromInfoDic(new string[] { "c", "channel_id", "domain", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "pf_build", "pf_ver", "platform_type", "pwd", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "timestamp", "udid", "uid", "user_id", "ver", "version", "sign" });

            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            var response = await Post(url, postParam, false, false, false);
            if (response.code == 0)
            {
                PlatfromInfos["access_key"] = response.access_key;
                PlatfromInfos["access_token"] = response.access_key;
                PlatfromInfos["expires"] = response.expires.ToString();
                if (string.IsNullOrEmpty(response.uid))
                    PlatfromInfos["uid"] = response.uid.ToString();
            }
            return response;
        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<ApiClientLoginRes> ApiClientLogin(string user, string pwd, string rsa, string hash, string access_key = null)
        {
            return await new ApiClientLoginReq(this).Send(user, pwd, rsa, hash, access_key);
        }
    }
}