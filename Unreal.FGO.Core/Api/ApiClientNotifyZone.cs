using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientNotifyZoneRes : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string result { get; set; }
    }

    public class ApiClientNotifyZoneReq : BaseRequest<ApiClientNotifyZoneRes>
    {
        public ApiClientNotifyZoneReq(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override async Task<ApiClientNotifyZoneRes> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>() {
                    { "version", "1" }
            };
            var url = "https://pinterface.biligame.net/api/client/notify.zone";
            Dictionary<string, string> postParam = null;
            replaceInfos["user_id"] = PlatfromInfos["uid"];
            postParam = getPlatfromInfoDic(new string[] { "c", "channel_id", "domain", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "pf_build", "pf_ver", "platform_type", "role_id", "role_name", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "server_name", "timestamp", "udid", "uid", "user_id", "ver", "version", "sign" });
            var response = await Post(url, postParam, false, false, false);
            if (response.code == 0)
            {
            }
            return response;
        }
    }
}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<ApiClientNotifyZoneRes> ApiClientNotifyZone()
        {
            return await new ApiClientNotifyZoneReq(this).Send();
        }
    }
}