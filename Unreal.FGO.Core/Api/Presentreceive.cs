using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
namespace Unreal.FGO.Core.Api
{
    public class PresentreceiveGetsvts
    {
        public string userSvtId { get; set; }
        public string isNew { get; set; }
    }
    public class PresentreceiveSuccess
    {
        public string isOverflow { get; set; }
        public List<PresentreceiveGetsvts> getSvts { get; set; }
    }
    public class PresentreceiveResponse
    {
        public string resCode { get; set; }
        public PresentreceiveSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class PresentreceiveUsersvt
    {
        public string userId { get; set; }
        public string svtId { get; set; }
        public int limitCount { get; set; }
        public string lv { get; set; }
        public int dispLimitCount { get; set; }
        public int imageLimitCount { get; set; }
        public int exp { get; set; }
        public int skillLv1 { get; set; }
        public int skillLv2 { get; set; }
        public int skillLv3 { get; set; }
        public int status { get; set; }
        public int treasureDeviceLv1 { get; set; }
        public int treasureDeviceLv2 { get; set; }
        public int treasureDeviceLv3 { get; set; }
        public int equipTargetId1 { get; set; }
        public int adjustHp { get; set; }
        public int adjustAtk { get; set; }
        public int updatedAt { get; set; }
        public int createdAt { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public int treasureDeviceId1 { get; set; }
        public int treasureDeviceId2 { get; set; }
        public int treasureDeviceId3 { get; set; }
        public int selectTreasureDeviceIdx { get; set; }
        public string id { get; set; }
    }
    public class PresentreceiveUseritem
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public string num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class PresentreceiveUsergame
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
        public int freeStone { get; set; }
        public int chargeStone { get; set; }
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
        public int stone { get; set; }
        public int friendKeep { get; set; }
        public int svtKeep { get; set; }
        public int svtEquipKeep { get; set; }
    }
    public class PresentreceiveEventmissionannounce
    {
    }
    public class PresentreceiveUpdated
    {
        public List<PresentreceiveUsersvt> userSvt { get; set; }
        public List<HomeUseritem> userItem { get; set; }
        public List<HomeUsergame> userGame { get; set; }
        public List<PresentreceiveEventmissionannounce> eventMissionAnnounce { get; set; }
    }
    public class PresentreceiveReplaced
    {
        public List<PresentreceiveEventmissionannounce> userPresentBox { get; set; }
    }
    public class PresentreceiveCache
    {
        public PresentreceiveUpdated updated { get; set; }
        public PresentreceiveReplaced replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class PresentreceiveRes : BaseResponse
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
        public List<PresentreceiveResponse> response { get; set; }
        public PresentreceiveCache cache { get; set; }
    }

    public class PresentreceiveReq : BaseRequest<PresentreceiveRes>
    {
        public PresentreceiveReq(ServerApi api) : base(api)
        {
        }

        public override async Task<PresentreceiveRes> Send(params string[] args)
        {
            if (args.Length == 0)
                return new PresentreceiveRes() { response = new List<PresentreceiveResponse>() { new PresentreceiveResponse() { resCode = "00" } } };
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["a"] = args[0];
            PlatfromInfos["key"] = "presentreceive";
            PlatfromInfos["presentIds"] = JsonConvert.SerializeObject(args).Replace("\"", "");
            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "a", "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "presentIds", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                if (response.cache != null && response.cache.updated != null)
                {
                    if (response.cache.updated.userGame != null && response.cache.updated.userGame.Count > 0)
                        serverApi.userGame = response.cache.updated.userGame[0];

                    if (response.cache.updated.userItem != null && response.cache.updated.userItem.Count > 0)
                        serverApi.UpdateItem(response.cache.updated.userItem);

                }
            }
            return response;
        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public List<ToploginUserdeck> userDeck { get; set; }

        public async Task<PresentreceiveRes> Presentreceive(params string[] ids)
        {
            return await new PresentreceiveReq(this).Send(ids);
        }
    }
}