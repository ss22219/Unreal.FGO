using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class HomeSuccess
    {
    }
    public class HomeResponse
    {
        public string resCode { get; set; }
        public HomeSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class HomeUserboxgacha
    {
        public string userId { get; set; }
        public string boxGachaId { get; set; }
        public string resetNum { get; set; }
        public int drawNum { get; set; }
        public string isReset { get; set; }
        public int boxIndex { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUsergame
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
        public int stone { get; set; }
        public int friendKeep { get; set; }
        public int svtKeep { get; set; }
        public int svtEquipKeep { get; set; }
    }
    public class HomeTblusergame
    {
        public string userId { get; set; }
        public string friendPoint { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUseritem
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public int num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUsersvtcollection
    {
        public string userId { get; set; }
        public string svtId { get; set; }
        public string status { get; set; }
        public string maxLv { get; set; }
        public string maxHp { get; set; }
        public string maxAtk { get; set; }
        public string maxLimitCount { get; set; }
        public string skillLv1 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string treasureDeviceLv1 { get; set; }
        public string treasureDeviceLv2 { get; set; }
        public string treasureDeviceLv3 { get; set; }
        public string friendship { get; set; }
        public string friendshipRank { get; set; }
        public string voicePlayed { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUsersvt
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
        public string equipTargetId1 { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string isLock { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
    }
    public class HomeEquiptarget1
    {
        public string userId { get; set; }
        public string userSvtId { get; set; }
        public string svtId { get; set; }
        public string limitCount { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public int skillId1 { get; set; }
        public string skillLv1 { get; set; }
        public int updatedAt { get; set; }
    }
    public class HomeUsersvtleader
    {
        public string userId { get; set; }
        public string classId { get; set; }
        public string userSvtId { get; set; }
        public string svtId { get; set; }
        public string limitCount { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public string hp { get; set; }
        public string atk { get; set; }
        public string adjustHp { get; set; }
        public string adjustAtk { get; set; }
        public string skillId1 { get; set; }
        public string skillId2 { get; set; }
        public string skillId3 { get; set; }
        public string skillLv1 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string treasureDeviceId { get; set; }
        public string treasureDeviceLv { get; set; }
        public HomeEquiptarget1 equipTarget1 { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeSvts
    {
        public int id { get; set; }
        public int userSvtId { get; set; }
        public int userId { get; set; }
        public List<string> userSvtEquipIds { get; set; }
        public string isFollowerSvt { get; set; }
    }
    public class HomeDeckinfo
    {
        public List<HomeSvts> svts { get; set; }
    }
    public class HomeUserdeck
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string deckNo { get; set; }
        public string name { get; set; }
        public string cost { get; set; }
        public string deckInfoJson { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
        public HomeDeckinfo deckInfo { get; set; }
    }
    public class HomeUserequip
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string equipId { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUserquest
    {
        public string userId { get; set; }
        public string questId { get; set; }
        public int questPhase { get; set; }
        public int clearNum { get; set; }
        public string isEternalOpen { get; set; }
        public string keyExpireAt { get; set; }
        public string keyCountRemain { get; set; }
        public string isNew { get; set; }
        public string lastStartedAt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUsergacha
    {
        public string userId { get; set; }
        public string gachaId { get; set; }
        public string num { get; set; }
        public string tenNum { get; set; }
        public string freeDrawAt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeReplaced
    {
        public List<HomeUserpresentbox> userPresentBox { get; set; }
        public List<HomeUsergame> userGame { get; set; }
        public List<HomeTblusergame> tblUserGame { get; set; }
        public List<HomeUseritem> userItem { get; set; }
        public List<HomeUsersvtcollection> userSvtCollection { get; set; }
        public List<ToploginUsersvt> userSvt { get; set; }
        public List<HomeUsersvtleader> userSvtLeader { get; set; }
        public List<ToploginUserdeck> userDeck { get; set; }
        public List<HomeUserequip> userEquip { get; set; }
        public List<HomeUserquest> userQuest { get; set; }
        public List<HomeUsergacha> userGacha { get; set; }
    }
    public class HomeUserlogin
    {
        public string userId { get; set; }
        public string seqLoginCount { get; set; }
        public string totalLoginCount { get; set; }
        public string lastLoginAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeTblfriend
    {
        public string userId { get; set; }
        public string friendId { get; set; }
        public string status { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeOtherusergame
    {
        public string id { get; set; }
        public string userId { get; set; }
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
        public List<HomeUsersvtleader> userSvtLeaderHash { get; set; }
        public string userName { get; set; }
        public string userLv { get; set; }
    }
    public class HomeUsershop
    {
        public string userId { get; set; }
        public string shopId { get; set; }
        public string num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class HomeUserevent
    {
        public string userId { get; set; }
        public string eventId { get; set; }
        public string value { get; set; }
        public string rank { get; set; }
    }
    public class HomeFollowerinfo
    {
        public string userId { get; set; }
        public int type { get; set; }
        public List<HomeUsersvtleader> userSvtLeaderHash { get; set; }
        public string userName { get; set; }
        public string userLv { get; set; }
    }
    public class HomeUserfollower
    {
        public string userId { get; set; }
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
        public long expireAt { get; set; }
        public string isDelete { get; set; }
        public List<HomeFollowerinfo> followerInfo { get; set; }
    }
    public class HomeUserpresentbox
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
    public class HomeUpdated
    {
        public List<HomeUserboxgacha> userBoxGacha { get; set; }
        public List<HomeUserpresentbox> userPresentBox { get; set; }
        public List<HomeUserlogin> userLogin { get; set; }
        public List<HomeTblfriend> tblFriend { get; set; }
        public List<HomeOtherusergame> otherUserGame { get; set; }
        public List<HomeUsershop> userShop { get; set; }
        public List<HomeUserevent> userEvent { get; set; }
        public List<HomeUserfollower> userFollower { get; set; }
    }
    public class HomeCache
    {
        public HomeReplaced replaced { get; set; }
        public HomeUpdated updated { get; set; }
        public int serverTime { get; set; }
    }
    public class HomeRes : BaseResponse
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
        public List<HomeResponse> response { get; set; }
        public HomeCache cache { get; set; }
    }

    public class HomeReq : BaseRequest<HomeRes>
    {
        public HomeReq(ServerApi api) : base(api)
        {
        }

        public override async Task<HomeRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "home";
            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                if (response.cache != null && response.cache.replaced != null)
                {
                    if (response.cache.replaced.userGame != null && response.cache.replaced.userGame.Count > 0)
                        serverApi.userGame = response.cache.replaced.userGame[0];

                    if (response.cache.replaced.userQuest != null && response.cache.replaced.userQuest.Count > 0)
                        serverApi.userQuest = response.cache.replaced.userQuest;

                    if (response.cache.replaced.userDeck != null && response.cache.replaced.userDeck.Count > 0)
                        serverApi.userDeck = response.cache.replaced.userDeck;

                    if (response.cache.replaced.userItem != null && response.cache.replaced.userItem.Count > 0)
                        serverApi.userItem = response.cache.replaced.userItem;

                    if (response.cache.replaced.userSvt != null)
                        serverApi.userSvt = response.cache.replaced.userSvt;
                }

                if (response.cache.updated != null && response.cache.updated.userFollower != null && response.cache.updated.userFollower.Count > 0)
                {
                    serverApi.userFollower = response.cache.updated.userFollower;
                }
                if (response.cache.updated != null && response.cache.updated.tblFriend != null && response.cache.updated.tblFriend.Count > 0)
                {
                    serverApi.UpdateFriend(response.cache.updated.tblFriend);
                }
                if (response.cache.updated != null && response.cache.updated.userBoxGacha != null)
                {
                    serverApi.userBoxGacha = response.cache.updated.userBoxGacha;
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
        public async Task<HomeRes> Home()
        {
            return await new HomeReq(this).Send();
        }
    }
}