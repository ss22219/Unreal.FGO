using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unreal.FGO.Core.Api
{
    public class DecksetupSuccess
    {
    }
    public class DecksetupResponse
    {
        public string resCode { get; set; }
        public DecksetupSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class DecksetupUsersvt
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string svtId { get; set; }
        public string limitCount { get; set; }
        public string dispLimitCount { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public string adjustHp { get; set; }
        public string adjustAtk { get; set; }
        public string status { get; set; }
        public string skillLv1 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string treasureDeviceLv1 { get; set; }
        public string treasureDeviceLv2 { get; set; }
        public string treasureDeviceLv3 { get; set; }
        public string selectTreasureDeviceIdx { get; set; }
        public int equipTargetId1 { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string isLock { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
    }
    public class DecksetupSvts
    {
        public int id { get; set; }
        public int userSvtId { get; set; }
        public int userId { get; set; }
        public List<string> userSvtEquipIds { get; set; }
        public string isFollowerSvt { get; set; }
    }
    public class DecksetupDeckinfo
    {
        public List<DecksetupSvts> svts { get; set; }
    }
    public class DecksetupUserdeck
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string deckNo { get; set; }
        public string name { get; set; }
        public int cost { get; set; }
        public string deckInfoJson { get; set; }
        public int updatedAt { get; set; }
        public string createdAt { get; set; }
        public DecksetupDeckinfo deckInfo { get; set; }
    }
    public class DecksetupUsergame
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
        public int activeDeckId { get; set; }
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
        public int stone { get; set; }
        public int friendKeep { get; set; }
        public int svtKeep { get; set; }
        public int svtEquipKeep { get; set; }
    }
    public class DecksetupUpdated
    {
        public List<DecksetupUsersvt> userSvt { get; set; }
        public List<DecksetupUserdeck> userDeck { get; set; }
        public List<DecksetupUsergame> userGame { get; set; }
    }
    public class DecksetupCache
    {
        public DecksetupUpdated updated { get; set; }
        public DecksetupSuccess replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class DecksetupRes : BaseResponse
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
        public List<DecksetupResponse> response { get; set; }
        //public DecksetupCache cache { get; set; }
    }

    public class DecksetupReq : BaseRequest<DecksetupRes>
    {
        public DecksetupReq(ServerApi api) : base(api)
        {
        }

        public override async Task<DecksetupRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "decksetup";
            PlatfromInfos["mainDeckId"] = args[0];
            PlatfromInfos["activeDeckId"] = args[0];
            PlatfromInfos["userDeck"] = args[1].Replace("null", "0").Replace("\"false\"", "false").Replace("\"0\"", "0");
            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "mainDeckId", "activeDeckId", "userDeck", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "dataVer" });
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
        public async Task<DecksetupRes> Decksetup(List<ToploginUserdeck> deck)
        {
            return await new DecksetupReq(this).Send(deck[0].id.ToString(), JsonConvert.SerializeObject(deck));
        }
    }
}