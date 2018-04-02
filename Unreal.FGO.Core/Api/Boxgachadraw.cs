using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class BoxgachadrawSuccess
    {
        public List<string> giftIds { get; set; }
        public List<string> resultNumbers { get; set; }
        public List<string> lastAccessTime { get; set; }
    }
    public class BoxgachadrawResponse
    {
        public string resCode { get; set; }
        public BoxgachadrawSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public string isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class BoxgachadrawUserboxgachadeck
    {
        public string userId { get; set; }
        public string boxGachaId { get; set; }
        public string boxGachaBaseNo { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BoxgachadrawUserpresentbox
    {
        public string receiveUserId { get; set; }
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
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
        public string presentId { get; set; }
    }
    public class BoxgachadrawTblusergame
    {
        public string userId { get; set; }
        public string friendPoint { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BoxgachadrawUsergame
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
        public string tutorial1 { get; set; }
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
        public string friendKeep { get; set; }
        public string svtKeep { get; set; }
        public string svtEquipKeep { get; set; }
    }
    public class BoxgachadrawBoxgachahistory
    {
        public string boxGachaId { get; set; }
        public List<string> numbers { get; set; }
    }
    public class BoxgachadrawUseritem
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public string num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BoxgachadrawUserboxgacha
    {
        public string userId { get; set; }
        public string boxGachaId { get; set; }
        public string resetNum { get; set; }
        public int drawNum { get; set; }
        public bool isReset { get; set; }
        public int boxIndex { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
        public string IsNewHash { get; set; }
    }
    public class BoxgachadrawUpdated
    {
        public List<BoxgachadrawUserboxgachadeck> userBoxGachaDeck { get; set; }
        public List<BoxgachadrawUserpresentbox> userPresentBox { get; set; }
        public List<BoxgachadrawTblusergame> tblUserGame { get; set; }
        public List<BoxgachadrawUsergame> userGame { get; set; }
        public List<BoxgachadrawBoxgachahistory> boxGachaHistory { get; set; }
        public List<HomeUseritem> userItem { get; set; }
        public List<BoxgachadrawUserboxgacha> userBoxGacha { get; set; }
    }
    public class BoxgachadrawReplaced
    {
    }
    public class BoxgachadrawCache
    {
        public BoxgachadrawUpdated updated { get; set; }
        public string serverTime { get; set; }
        public BoxgachadrawReplaced replaced { get; set; }
    }
    public class BoxgachadrawRes : BaseResponse
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
        public List<BoxgachadrawResponse> response { get; set; }
        public BoxgachadrawCache cache { get; set; }
    }

    public class BoxgachadrawReq : BaseRequest<BoxgachadrawRes>
    {
        public BoxgachadrawReq(ServerApi api) : base(api)
        {
        }

        public override async Task<BoxgachadrawRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "boxgachadraw";

            PlatfromInfos["boxGachaId"] = args[0];
            PlatfromInfos["num"] = args[1];

            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "boxGachaId", "num", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                if (response.cache.updated.userItem != null)
                    this.serverApi.UpdateItem(response.cache.updated.userItem);
            }
            return response;

        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<BoxgachadrawRes> Boxgachadraw(string boxGachaId,int num)
        {
            return await new BoxgachadrawReq(this).Send(boxGachaId, num.ToString());
        }
    }
}