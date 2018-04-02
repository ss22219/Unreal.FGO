using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class PresentlistSuccess
    {
    }
    public class PresentlistResponse
    {
        public string resCode { get; set; }
        public PresentlistSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class PresentlistUserpresentbox
    {
        public string receiveUserId { get; set; }
        public string presentId { get; set; }
        public string isAuto { get; set; }
        public string messageRefType { get; set; }
        public string messageId { get; set; }
        public string message { get; set; }
        public string args { get; set; }
        public string fromType { get; set; }
        public string giftType { get; set; }
        public string objectId { get; set; }
        public string num { get; set; }
        public string limitCount { get; set; }
        public string lv { get; set; }
        public string status { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class PresentlistReplaced
    {
        public List<PresentlistUserpresentbox> userPresentBox { get; set; }
    }
    public class PresentlistCache
    {
        public PresentlistReplaced replaced { get; set; }
        public PresentlistSuccess updated { get; set; }
        public int serverTime { get; set; }
    }
    public class PresentlistRes : BaseResponse
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
        public List<PresentlistResponse> response { get; set; }
        public PresentlistCache cache { get; set; }
    }

    public class PresentlistReq : BaseRequest<PresentlistRes>
    {
        public PresentlistReq(ServerApi api) : base(api)
        {
        }

        public override async Task<PresentlistRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "presentlist";
            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "dataVer" });
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
        public List<HomeUserfollower> userFollower { get; set; }

        public async Task<PresentlistRes> Presentlist()
        {
            return await new PresentlistReq(this).Send();
        }
    }
}