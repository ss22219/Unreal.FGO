using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class MemberResponse
    {
        public string resCode { get; set; }
        public success success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
    }
    public class MemberCache
    {
        public long serverTime { get; set; }
    }
    public class MemberRes : BaseResponse
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
        public List<MemberResponse> response { get; set; }
        public MemberCache cache { get; set; }
    }

    public class MemberReq : BaseRequest<MemberRes>
    {
        public MemberReq(ServerApi api) : base(api)
        {
        }

        public override async Task<MemberRes> Send(params string[] args)
        {
            var url = "/rongame_beta//rgfate/60_member/member.php";
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "deviceid", "t", "v", "s", "mac", "os", "ptype", "imei", "username", "type", "password", "rksdkid", "rkchannel", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "version", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                serverApi.serverOffsetTime = (response.cache.serverTime - TimetampSecond);
            }
            return response;
        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {

        public async Task<MemberRes> Member()
        {
            return await new MemberReq(this).Send();
        }
    }
}