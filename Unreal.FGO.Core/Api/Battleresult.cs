using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Unreal.FGO.Core.Api
{
    public class BattleresultActionItem
    {
        public int uid { get; set; }
        public int ty { get; set; }
    }

    public class BattleresultAction
    {
        public BattleresultActionItem[] logs { get; set; }
    }

    public class BattleresultSvts
    {
        public string id { get; set; }
        public string userSvtId { get; set; }
    }
    public class BattleresultOldusersvtcollection
    {
        public string userId { get; set; }
        public string svtId { get; set; }
        public string status { get; set; }
        public string friendship { get; set; }
        public string friendshipRank { get; set; }
    }
    public class BattleresultOldusergame
    {
        public string userId { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public string qp { get; set; }
        public string actMax { get; set; }
        public string genderType { get; set; }
        public string costMax { get; set; }
        public int friendKeep { get; set; }
    }
    public class BattleresultOlduserequip
    {
        public string id { get; set; }
        public string equipId { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
    }
    public class BattleresultOlduserquest
    {
        public string userId { get; set; }
        public string questId { get; set; }
        public string questPhase { get; set; }
        public string clearNum { get; set; }
    }
    public class BattleresultOlduserevent
    {
        public string userId { get; set; }
        public string eventId { get; set; }
        public string value { get; set; }
        public string rank { get; set; }
    }
    public class BattleresultResultdropinfos
    {
        public string type { get; set; }
        public string objectId { get; set; }
        public int num { get; set; }
        public int limitCount { get; set; }
        public string isNew { get; set; }
        public string userSvtId { get; set; }
        public string priority { get; set; }
    }
    public class BattleresultSuccess
    {
        public string battleId { get; set; }
        public string battleResult { get; set; }
        public string followerClassId { get; set; }
        public string phaseClearQp { get; set; }
        public string followerId { get; set; }
        public string followerType { get; set; }
        public string followerStatus { get; set; }
        public List<BattleresultOldusersvtcollection> oldUserSvtCollection { get; set; }
        public List<BattleresultOldusergame> oldUserGame { get; set; }
        public List<BattleresultOlduserequip> oldUserEquip { get; set; }
        public List<BattleresultOlduserquest> oldUserQuest { get; set; }
        public List<BattleresultOlduserevent> oldUserEvent { get; set; }
        public List<BattleresultResultdropinfos> resultDropInfos { get; set; }
        public string eventEndMessage { get; set; }
        public string eventEndTitle { get; set; }
        public string eventId { get; set; }
        public string eventOpenStatus { get; set; }
        public string createTime { get; set; }
    }
    public class BattleresultResponse
    {
        public string resCode { get; set; }
        //public BattleresultSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class BattleresultUserquest
    {
        public string userId { get; set; }
        public string questId { get; set; }
        public string questPhase { get; set; }
        public int clearNum { get; set; }
        public string isEternalOpen { get; set; }
        public string keyExpireAt { get; set; }
        public string keyCountRemain { get; set; }
        public string isNew { get; set; }
        public string lastStartedAt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattleresultUserevent
    {
        public string userId { get; set; }
        public string eventId { get; set; }
        public string value { get; set; }
        public string rank { get; set; }
        public string IsNewHash { get; set; }
    }
    public class BattleresultUserequip
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string equipId { get; set; }
        public string lv { get; set; }
        public int exp { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattleresultUsersvtcollection
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
    public class BattleresultUseritem
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public string num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattleresultUsergame
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
        public int exp { get; set; }
        public int qp { get; set; }
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
        public string stone { get; set; }
        public string friendKeep { get; set; }
        public string svtKeep { get; set; }
        public string svtEquipKeep { get; set; }
    }
    public class BattleresultTblusergame
    {
        public string userId { get; set; }
        public string friendPoint { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattleresultEquiptarget1
    {
        public string userId { get; set; }
        public string userSvtId { get; set; }
        public string svtId { get; set; }
        public string limitCount { get; set; }
        public string lv { get; set; }
        public string exp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public string skillId1 { get; set; }
        public string skillLv1 { get; set; }
        public long updatedAt { get; set; }
    }
    public class BattleresultUsersvtleaderhash
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
        public BattleresultEquiptarget1 equipTarget1 { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattleresultOtherusergame
    {
        public string userId { get; set; }
        public string userLv { get; set; }
        public string userName { get; set; }
        public string friendCode { get; set; }
        public List<BattleresultUsersvtleaderhash> userSvtLeaderHash { get; set; }
    }
    public class BattleresultFollowerinfo
    {
        public string userId { get; set; }
        public string type { get; set; }
        public List<BattleresultUsersvtleaderhash> userSvtLeaderHash { get; set; }
        public string userName { get; set; }
        public string userLv { get; set; }
    }
    public class BattleresultUserfollower
    {
        public string userId { get; set; }
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
        public long expireAt { get; set; }
        public string isDelete { get; set; }
        public List<BattleresultFollowerinfo> followerInfo { get; set; }
    }
    public class BattleresultEventmissionannounce
    {
    }
    public class BattleresultUpdated
    {
        public List<HomeUserquest> userQuest { get; set; }
        public List<BattleresultUserevent> userEvent { get; set; }
        public List<BattleresultUserequip> userEquip { get; set; }
        public List<BattleresultUsersvtcollection> userSvtCollection { get; set; }
        public List<HomeUseritem> userItem { get; set; }
        public List<HomeUsergame> userGame { get; set; }
        public List<BattleresultTblusergame> tblUserGame { get; set; }
        public List<BattleresultOtherusergame> otherUserGame { get; set; }
        public List<HomeUserfollower> userFollower { get; set; }
        public List<BattleresultEventmissionannounce> eventMissionAnnounce { get; set; }
    }
    public class BattleresultCache
    {
        public BattleresultUpdated updated { get; set; }
        public long serverTime { get; set; }
    }
    public class BattleresultRes : BaseResponse
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
        public List<BattleresultResponse> response { get; set; }
        public BattleresultCache cache { get; set; }
    }

    public class BattleresultReq : BaseRequest<BattleresultRes>
    {
        public BattleresultReq(ServerApi api) : base(api)
        {
        }

        public override async Task<BattleresultRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "battleresult";
            replaceInfos = new Dictionary<string, string>()
            {
                { "scores",""},
                { "battleId",args[0] },
                { "action",args[1] },
                { "voicePlayedList",args[2] },
                { "battleMode","1" },
                { "battleResult","1" },
            };
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "battleId", "scores", "action", "voicePlayedList", "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "battleResult", "battleMode", "userAgent", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                try
                {
                    if (response.cache != null && response.cache.updated != null)
                    {
                        if (response.cache.updated.userGame != null && response.cache.updated.userGame.Count > 0)
                            serverApi.userGame = response.cache.updated.userGame[0];

                        if (response.cache.updated.userQuest != null && response.cache.updated.userQuest.Count > 0)
                            serverApi.UpdateQuest(response.cache.updated.userQuest);

                        if (response.cache.updated.userFollower != null && response.cache.updated.userFollower.Count > 0)
                        {
                            serverApi.UpdateFollower(response.cache.updated.userFollower);
                        }

                        if (response.cache.updated.userItem != null && response.cache.updated.userItem.Count > 0)
                            serverApi.UpdateItem(response.cache.updated.userItem);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex);
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
        public async Task<BattleresultRes> Battleresult(string battleId, BattleresultActionItem[] action, string voicePlayedList = "[]")
        {
            var actionStr = JsonConvert.SerializeObject(new BattleresultAction() { logs = action });
            return await new BattleresultReq(this).Send(battleId, actionStr, voicePlayedList);
        }

        public async Task<BattleresultRes> Battleresult(string battleId, string action, string voicePlayedList = "[]")
        {
            return await new BattleresultReq(this).Send(battleId, action, voicePlayedList);
        }

        public void UpdateFollower(List<HomeUserfollower> followers)
        {
            if (userFollower == null)
            {
                userFollower = followers;
                return;
            }
            foreach (var follower in followers)
            {
                var index = userFollower.FindIndex(f => f.followerInfo[0].userId == follower.followerInfo[0].userId);
                if (index != -1)
                {
                    userFollower[index] = follower;
                }
                else
                    userFollower.Add(follower);
            }
        }
    }
}