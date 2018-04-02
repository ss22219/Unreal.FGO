using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class LogintomembercenterResponse
    {
        public string resCode { get; set; }
        public fail fail { get; set; }
        public LogintomembercenterSuccess success { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }

    public class LogintomembercenterSuccess
    {
        public string rguid { get; set; }
        public string rgusk { get; set; }
        public int rgtype { get; set; }
        public string version { get; set; }
        public string dateVer { get; set; }
        public string dataVer { get; set; }
    }

    public class LogintomembercenterRes : BaseResponse
    {
        public override int code
        {
            get
            {
                return response != null && response.Count > 0 ? int.Parse(response[0].resCode) : 99;
            }

            set
            {
                base.code = value;
            }
        }
        public override string message
        {
            get
            {
                return response != null && response.Count > 0 && response[0].fail != null ? response[0].fail.title + " " + response[0].fail.detail : base.message;
            }

            set
            {
                base.message = value;
            }
        }
        public override string action
        {
            get
            {
                return response[0].fail.action;
            }

            set
            {
                base.action = value;
            }
        }
        public List<LogintomembercenterResponse> response { get; set; }
    }

    public class LogintomembercenterReq : BaseRequest<LogintomembercenterRes>
    {
        public LogintomembercenterReq(ServerApi api) : base(api)
        {
        }

        public override async Task<LogintomembercenterRes> Send(params string[] args)
        {
            var url = "/rongame_beta//rgfate/60_member/logintomembercenter.php";
            PlatfromInfos["type"] = "token";
            PlatfromInfos["version"] = PlatfromInfos["dataVer"];
            PlatfromInfos["dateVer"] = PlatfromInfos["dataVer"];

            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "deviceid", "t", "v", "s", "mac", "os", "ptype", "imei", "username", "type", "rkuid", "access_token", "rksdkid", "rkchannel", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "version", "dataVer" });

            var response = await Post(url, postParam);
            if (response.response != null && response.response.Count > 0 && !string.IsNullOrEmpty(response.response[0].usk))
            {
                PlatfromInfos["rgusk"] = response.response[0].usk;
                PlatfromInfos["rguid"] = response.response[0].success.rguid;
                PlatfromInfos["usk"] = response.response[0].usk;
                PlatfromInfos["dateVer"] = response.response[0].success.dateVer.ToString();
            }
            else
            {
                PlatfromInfos.Remove("rgusk");
                PlatfromInfos.Remove("rguid");
                PlatfromInfos.Remove("usk");
                PlatfromInfos.Remove("access_token");
                PlatfromInfos.Remove("access_key");
            }
            return response;
        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<LogintomembercenterRes> LoginToMemberCenter()
        {
            return await new LogintomembercenterReq(this).Send();
        }
    }
}