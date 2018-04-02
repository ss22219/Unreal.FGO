using System.Collections.Generic;
using System.Threading.Tasks;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientUserInfoRes : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string face { get; set; }
        public string s_face { get; set; }
        public int uid { get; set; }
        public string uname { get; set; }
    }

    public class ApiClientUserInfoReq : BaseRequest<ApiClientUserInfoRes>
    {
        public ApiClientUserInfoReq(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override async Task<ApiClientUserInfoRes> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>() {
                    { "version", "1" }
            };
            if (args != null && args.Length > 0 && args[0] != null)
            {
                replaceInfos["access_key"] = args[0];
            }
            var url = "https://pinterface.biligame.net/api/client/user.info";
            string getParam = null;
            if (!serverApi.ios)
                getParam = getPlatfromInfo(new string[] { "access_key", "uid", "server_id", "original_domain", "model", "pf_ver", "c", "domain_switch_count", "ver", "udid", "net", "sdk_log_type", "sdk_ver", "channel_id", "version", "sign", "timestamp", "sdk_type", "platform_type", "game_id", "domain", "operators", "ad_ext", "app_id", "merchant_id", "dp" });
            else
                getParam = getPlatfromInfo(new string[] { "access_key", "c", "channel_id", "domain", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "pf_build", "pf_ver", "platform_type", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "timestamp", "udid", "ver", "version", "sign" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;

            var response = await Get(url, false, false, false);
            if (response.code == 0)
            {
                PlatfromInfos["uname"] = response.uname;
                PlatfromInfos["face"] = response.face;
                PlatfromInfos["s_face"] = response.s_face;
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
        public async Task<ApiClientUserInfoRes> ApiClientUserInfo(string access_key = null)
        {
            return await new ApiClientUserInfoReq(this).Send(access_key);
        }
    }
}