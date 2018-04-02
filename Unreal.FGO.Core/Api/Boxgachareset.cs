using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class BoxgacharesetSuccess
    {
    }
    public class BoxgacharesetResponse
    {
        public string resCode { get; set; }
        public BoxgacharesetSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public string isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class BoxgacharesetUserboxgacha
    {
        public string userId { get; set; }
        public string boxGachaId { get; set; }
        public string resetNum { get; set; }
        public string drawNum { get; set; }
        public string isReset { get; set; }
        public int boxIndex { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BoxgacharesetUserboxgachadeck
    {
        public string userId { get; set; }
        public string boxGachaId { get; set; }
        public string boxGachaBaseNo { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BoxgacharesetBoxgachahistory
    {
        public string boxGachaId { get; set; }
    }
    public class BoxgacharesetUpdated
    {
        public List<BoxgacharesetUserboxgacha> userBoxGacha { get; set; }
        public List<BoxgacharesetUserboxgachadeck> userBoxGachaDeck { get; set; }
        public List<BoxgacharesetBoxgachahistory> boxGachaHistory { get; set; }
    }
    public class BoxgacharesetCache
    {
        public BoxgacharesetUpdated updated { get; set; }
        public string serverTime { get; set; }
        public BoxgacharesetSuccess replaced { get; set; }
    }
    public class BoxgacharesetRes : BaseResponse
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
        public List<BoxgacharesetResponse> response { get; set; }
        public BoxgacharesetCache cache { get; set; }
    }

    public class BoxgacharesetReq : BaseRequest<BoxgacharesetRes>
    {
        public BoxgacharesetReq(ServerApi api) : base(api)
        {
        }

        public override async Task<BoxgacharesetRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "boxgachareset";

            PlatfromInfos["boxGachaId"] = args[0];
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "boxGachaId", "dataVer" });
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
        public async Task<BoxgacharesetRes> Boxgachareset(string boxGachaId)
        {
            return await new BoxgacharesetReq(this).Send(boxGachaId);
        }
    }
}