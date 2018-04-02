using System.Collections.Generic;
using System.Threading.Tasks;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientRsaRes : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string hash { get; set; }
        public string rsa_key { get; set; }
    }

    public class ApiClientRsaReq : BaseRequest<ApiClientRsaRes>
    {
        public ApiClientRsaReq(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override Task<ApiClientRsaRes> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>() {
                    { "version", "1" }
            };
            var url = "https://pinterface.biligame.net/api/client/rsa";
            string getParam = null;
            if (!serverApi.ios)
                getParam = getPlatfromInfo(new string[] { "uid", "server_id", "original_domain", "model", "pf_ver", "c", "domain_switch_count", "ver", "udid", "net", "sdk_log_type", "sdk_ver", "channel_id", "version", "sign", "timestamp", "sdk_type", "platform_type", "game_id", "domain", "operators", "ad_ext", "app_id", "merchant_id", "dp" });
            else
                getParam = getPlatfromInfo(new string[] { "c", "channel_id", "domain", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "pf_build", "pf_ver", "platform_type", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "timestamp", "udid", "ver", "version", "sign" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            return Get(url, false, false, false);
        }
    }
}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<ApiClientRsaRes> ApiClientRsa()
        {
            return await new ApiClientRsaReq(this).Send();
        }
    }
}