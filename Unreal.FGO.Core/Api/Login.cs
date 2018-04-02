using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class announcement
    {
        public int id { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public int isshow { get; set; }
        public int isdel { get; set; }
        public string url { get; set; }
        public string imgurl { get; set; }
        public string addtime { get; set; }
        public int updatetime { get; set; }
    }
    public class LoginResponseSuccess
    {
        public string sguid { get; set; }
        public int level { get; set; }
        public int createTime { get; set; }
        public string nickname { get; set; }
        public string sgusk { get; set; }
        public int sgtype { get; set; }
        public string sgtag { get; set; }
        public string type { get; set; }
        public string platformManagement { get; set; }
        public List<announcement> announcement { get; set; }
    }

    public class LoginResponse
    {
        public string resCode { get; set; }
        public LoginResponseSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }

    public class LoginRes : BaseResponse
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
        public List<LoginResponse> response { get; set; }
    }

    public class LoginReq : BaseRequest<LoginRes>
    {
        public LoginReq(ServerApi api) : base(api)
        {
        }

        public override async Task<LoginRes> Send(params string[] args)
        {
            PlatfromInfos["type"] = "login";
            PlatfromInfos["nickname"] = PlatfromInfos["uname"];
            var url = "/rongame_beta//rgfate/60_1001/login.php";
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "deviceid", "os", "ptype", "rgsid", "rguid", "rgusk", "idfa", "v", "mac", "imei", "type", "nickname", "rkchannel", "assetbundleFolder", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "t", "s", "rksdkid", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                PlatfromInfos["sgtag"] = response.response[0].success.sgtag;
                PlatfromInfos["nickname"] = response.response[0].success.nickname;
                PlatfromInfos["role_name"] = response.response[0].success.nickname;
                PlatfromInfos["_userId"] = response.response[0].success.sguid;
                PlatfromInfos["userId"] = response.response[0].success.sguid;
                PlatfromInfos["role_id"] = response.response[0].success.sguid;
            }
            return response;
        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<LoginRes> Login()
        {
            return await new LoginReq(this).Send();
        }
    }
}