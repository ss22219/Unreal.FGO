using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class TutorialsetSuccess
    {
    }
    public class TutorialsetResponse
    {
        public string resCode { get; set; }
        public TutorialsetSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public int isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class TutorialsetUsergame
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string usk { get; set; }
        public string appuid { get; set; }
        public string appname { get; set; }
        public string rksdkid { get; set; }
        public string rkchannel { get; set; }
        public string name { get; set; }
        public string birthDay { get; set; }
        public string actMax { get; set; }
        public string actRecoverAt { get; set; }
        public string carryOverActPoint { get; set; }
        public string genderType { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public string qp { get; set; }
        public string costMax { get; set; }
        public string friendCode { get; set; }
        public string favoriteUserSvtId { get; set; }
        public string friendKeepBase { get; set; }
        public string friendKeepAdjust { get; set; }
        public string commandSpellRecoverAt { get; set; }
        public string svtKeepBase { get; set; }
        public string svtKeepAdjust { get; set; }
        public string svtEquipKeepBase { get; set; }
        public string svtEquipKeepAdjust { get; set; }
        public string userEquipId { get; set; }
        public string freeStone { get; set; }
        public string chargeStone { get; set; }
        public string getPay { get; set; }
        public string mana { get; set; }
        public string mainDeckId { get; set; }
        public string activeDeckId { get; set; }
        public int tutorial1 { get; set; }
        public string tutorial2 { get; set; }
        public string tutorialProgress { get; set; }
        public string darkGachaNum { get; set; }
        public string mregtime { get; set; }
        public string zerotime { get; set; }
        public string sweepNum { get; set; }
        public string uflag { get; set; }
        public string deviceinfo { get; set; }
        public string md5str { get; set; }
        public string md5key { get; set; }
        public string regtime { get; set; }
        public string lasttime { get; set; }
        public string thawAt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
        public string rewardGetIds { get; set; }
        public string weekFriendPoint { get; set; }
        public string chargeRewardRecord { get; set; }
        public string firstgoldtime { get; set; }
        public string loginresettime { get; set; }
        public string last_ac { get; set; }
        public string last_ac_time { get; set; }
        public string appVer { get; set; }
        public string tint01 { get; set; }
        public string tint02 { get; set; }
        public string tint03 { get; set; }
        public string tint04 { get; set; }
        public string tint05 { get; set; }
        public string tvar01 { get; set; }
        public string tvar02 { get; set; }
        public string tvar03 { get; set; }
        public string tvar04 { get; set; }
        public string tvar05 { get; set; }
        public int stone { get; set; }
        public int friendKeep { get; set; }
        public int svtKeep { get; set; }
        public int svtEquipKeep { get; set; }
    }
    public class TutorialsetUpdated
    {
        public List<TutorialsetUsergame> userGame { get; set; }
    }
    public class TutorialsetCache
    {
        public TutorialsetUpdated updated { get; set; }
        public TutorialsetSuccess replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class TutorialsetRes : BaseResponse
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
        public List<TutorialsetResponse> response { get; set; }
        public TutorialsetCache cache { get; set; }
    }

    public class TutorialsetReq : BaseRequest<TutorialsetRes>
    {
        public TutorialsetReq(ServerApi api) : base(api)
        {
        }

        public override async Task<TutorialsetRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "tutorialset";
            PlatfromInfos["flagId"] = args[0];
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "flagId", "userAgent", "dataVer" });
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
        public async Task<TutorialsetRes> Tutorialset(string flagId)
        {
            return await new TutorialsetReq(this).Send(flagId);
        }
    }
}