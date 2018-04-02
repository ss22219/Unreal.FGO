using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class TutorialprogressSuccess
    {
    }
    public class TutorialprogressResponse
    {
        public string resCode { get; set; }
        public TutorialprogressSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public int isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class TutorialprogressCache
    {
        public TutorialprogressSuccess updated { get; set; }
        public TutorialprogressSuccess replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class TutorialprogressRes : BaseResponse
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
        public List<TutorialprogressResponse> response { get; set; }
        public TutorialprogressCache cache { get; set; }
    }

    public class TutorialprogressReq : BaseRequest<TutorialprogressRes>
    {
        public TutorialprogressReq(ServerApi api) : base(api)
        {
        }

        public override async Task<TutorialprogressRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "tutorialprogress";
            PlatfromInfos["tutorialprogress"] = args[0];
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "tutorialprogress", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
            }
            return response;

        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<TutorialprogressRes> Tutorialprogress(string tutorialprogress)
        {
            return await new TutorialprogressReq(this).Send(tutorialprogress);
        }
    }
}