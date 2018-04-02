using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.Core.Api
{
    public class ApiClientCreateRoleRes : BaseResponse
    {
        public string requestId { get; set; }
        public string timestamp { get; set; }
        public string access_key { get; set; }
        public string uid { get; set; }
        public long expires { get; set; }
    }

    public class ApiClientCreateRoleReq : BaseRequest<ApiClientCreateRoleRes>
    {
        public ApiClientCreateRoleReq(ServerApi api) : base(api)
        {
            httpClient = serverApi.BSGameSDKClient;
        }

        public override async Task<ApiClientCreateRoleRes> Send(params string[] args)
        {
            replaceInfos = new Dictionary<string, string>() {
                    { "version", "1" },
                    { "domain","pinterface.biligame.net"},
                    { "operators","4"}
            };
            replaceInfos["role"] = args[0];
            base.replaceInfos = replaceInfos;

            var url = "https://pinterface.biligame.net/api/client/createrole";
            string getParam = null;
            if (!serverApi.ios)
                getParam = getPlatfromInfo(new string[] { "model", "pf_ver", "ver", "net", "sdk_ver", "version", "timestamp", "game_id", "domain", "uid", "operators", "ad_ext", "merchant_id", "dp", "server_id", "c", "udid", "sdk_log_type", "channel_id", "sign", "role", "role_id", "sdk_type", "platform_type", "app_id" });
            else
                getParam = getPlatfromInfo(new string[] { "c", "channel_id", "domain", "dp", "game_id", "idfa", "merchant_id", "model", "net", "operators", "pf_build", "pf_ver", "platform_type", "role", "role_id", "sdk_log_type", "sdk_type", "sdk_ver", "server_id", "timestamp", "udid", "uid", "ver", "version", "sign" });

            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            var response = await Get(url, false, false, false);
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
        public async Task<ApiClientCreateRoleRes> ApiClientCreateRole(string role)
        {
            return await new ApiClientCreateRoleReq(this).Send(role);
        }
    }
}