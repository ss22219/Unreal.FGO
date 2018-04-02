using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unreal.FGO.Core;

namespace Unreal.FGO.Core.Api
{
    public class BattlesetupSuccess
    {
    }
    public class BattlesetupResponse
    {
        public string resCode { get; set; }
        public BattlesetupSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class BattlesetupUserquest
    {
        public string userId { get; set; }
        public string questId { get; set; }
        public string questPhase { get; set; }
        public string clearNum { get; set; }
        public string isEternalOpen { get; set; }
        public string keyExpireAt { get; set; }
        public string keyCountRemain { get; set; }
        public string isNew { get; set; }
        public long lastStartedAt { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class BattlesetupUsergame
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
        public long actRecoverAt { get; set; }
        public long carryOverActPoint { get; set; }
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
        public string stone { get; set; }
        public string friendKeep { get; set; }
        public string svtKeep { get; set; }
        public string svtEquipKeep { get; set; }
    }
    public class BattlesetupUpdated
    {
        public List<HomeUserquest> userQuest { get; set; }
        public List<HomeUsergame> userGame { get; set; }
    }

    public class BattlesetupMsteventdetail
    {
        public string eventId { get; set; }
        public string isBoxGacha { get; set; }
        public string isExchangeShop { get; set; }
        public string isEventPoint { get; set; }
        public string isRanking { get; set; }
        public string isBonusSkill { get; set; }
        public string isMission { get; set; }
        public string rewardPageBgId { get; set; }
        public string bgmId { get; set; }
        public string afterBgmId { get; set; }
        public string pointImageId { get; set; }
        public string guideImageId { get; set; }
        public object guideImageIds { get; set; }
        public string guideLimitCount { get; set; }
        public object guideLimitCounts { get; set; }
        public string condQuestId { get; set; }
        public string condMessage { get; set; }
        public object tutorialImageIds { get; set; }
    }
    public class BattlesetupResultinfo
    {
        public string rewardQp { get; set; }
        public string rewardExp { get; set; }
        public string rewardFriendShip { get; set; }
        public string rewardFriendPoint { get; set; }
        public string rewardUserEquipExp { get; set; }
    }
    public class BattlesetupMyusersvtequip
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string svtId { get; set; }
        public string lv { get; set; }
        public string limitCount { get; set; }
        public string dispLimitCount { get; set; }
        public string exp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public string skillLv1 { get; set; }
        public string skillId1 { get; set; }
        public string equipTargetId1 { get; set; }
        public string adjustHp { get; set; }
        public string adjustAtk { get; set; }
        public string parentSvtId { get; set; }
        public List<string> svtIndividuality { get; set; }
    }
    public class BattlesetupSvts
    {
        public string id { get; set; }
        public string userSvtId { get; set; }
        public string userId { get; set; }
        public string isFollowerSvt { get; set; }
        public List<string> userSvtEquipIds { get; set; }
        public string uniqueId { get; set; }
    }
    public class BattlesetupMydeck
    {
        public List<BattlesetupSvts> svts { get; set; }
    }
    public class BattlesetupMstskilllv
    {
        public string skillId { get; set; }
        public int lv { get; set; }
        public string chargeTurn { get; set; }
        public string funcId { get; set; }
        public List<string> vals { get; set; }
        public string skillDetailId { get; set; }
        public string priority { get; set; }
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
    }
    public class BattlesetupSvtskilllvcontainers
    {
        public BattlesetupMstskilllv mstSkillLv { get; set; }
    }
    public class BattlesetupPassiveskillcontainer
    {
        public List<BattlesetupSvtskilllvcontainers> svtSkillLvContainers { get; set; }
    }
    public class BattlesetupEnemyusersvt
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string svtId { get; set; }
        public int lv { get; set; }
        public int limitCount { get; set; }
        public int dispLimitCount { get; set; }
        public int exp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public string skillLv1 { get; set; }
        public string skillId1 { get; set; }
        public string equipTargetId1 { get; set; }
        public List<string> individuality { get; set; }
        public List<string> classPassive { get; set; }
        public int adjustHp { get; set; }
        public int adjustAtk { get; set; }
        public string skillId2 { get; set; }
        public string skillId3 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string treasureDeviceId { get; set; }
        public string treasureDeviceLv { get; set; }
        public string criticalRate { get; set; }
        public string starRate { get; set; }
        public string tdRate { get; set; }
        public string deathRate { get; set; }
        public string aiId { get; set; }
        public string chargeTurn { get; set; }
        public string actPriority { get; set; }
        public int maxActNum { get; set; }
        public string hpGaugeType { get; set; }
        public string npcSvtType { get; set; }
        public string displayType { get; set; }
    }
    public class BattlesetupUsersvt
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string svtId { get; set; }
        public string lv { get; set; }
        public string limitCount { get; set; }
        public int dispLimitCount { get; set; }
        public string exp { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public string skillId1 { get; set; }
        public string skillId2 { get; set; }
        public string skillId3 { get; set; }
        public string skillLv1 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string treasureDeviceId { get; set; }
        public string treasureDeviceLv { get; set; }
        public int status { get; set; }
        public List<string> classPassive { get; set; }
        public string adjustHp { get; set; }
        public string adjustAtk { get; set; }
        public int deathRate { get; set; }
        public List<string> individuality { get; set; }
        public string equipTargetId1 { get; set; }
    }
    public class BattlesetupBattleinfo
    {
        public string appVer { get; set; }
        public string dataVer { get; set; }
        public List<BattlesetupMyusersvtequip> myUserSvtEquip { get; set; }
        public BattlesetupMydeck myDeck { get; set; }
        public BattlesetupPassiveskillcontainer passiveSkillContainer { get; set; }
        public List<BattlesetupEnemyusersvt> enemyUserSvt { get; set; }
        public List<BattlesetupMydeck> enemyDeck { get; set; }
        public BattlesetupSuccess transformDeck { get; set; }
        public string userEquipId { get; set; }
        public List<BattlesetupUsersvt> userSvt { get; set; }
    }
    public class BattlesetupBattle
    {
        public string userId { get; set; }
        public string questId { get; set; }
        public string questPhase { get; set; }
        public string battleType { get; set; }
        public string followerId { get; set; }
        public string followerType { get; set; }
        public string seed { get; set; }
        public string isCompress { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public long createdAt { get; set; }
        public long updatedAt { get; set; }
        //public List<BattlesetupMsteventdetail> mstEventDetail { get; set; }
        public string rankingEventId { get; set; }
        public string eventId { get; set; }
        public string followerClassId { get; set; }
        public BattlesetupResultinfo resultInfo { get; set; }
        public BattlesetupBattleinfo battleInfo { get; set; }
        public string id { get; set; }
    }
    public class BattlesetupReplaced
    {
        public List<BattlesetupBattle> battle { get; set; }
    }
    public class BattlesetupCache
    {
        public BattlesetupUpdated updated { get; set; }
        public BattlesetupReplaced replaced { get; set; }
        public long serverTime { get; set; }
    }
    public class BattlesetupRes : BaseResponse
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
        public List<BattlesetupResponse> response { get; set; }
        public BattlesetupCache cache { get; set; }
    }

    public class BattlesetupReq : BaseRequest<BattlesetupRes>
    {
        public BattlesetupReq(ServerApi api) : base(api)
        {
        }

        public override async Task<BattlesetupRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "battlesetup";
            replaceInfos = new Dictionary<string, string>()
            {
                { "activeDeckId",args[0] },
                { "followerId",args[1] },
                { "questId",args[2] },
                { "questPhase",args[3] },
                { "followerClassId",args[4] },
                { "battleMode","1" },
            };
            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "activeDeckId", "followerId", "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "questId", "questPhase", "followerClassId", "battleMode", "userAgent", "dataVer" });
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
        public async Task<BattlesetupRes> Battlesetup(string activeDeckId, string followerId, string questId, string questPhase = "1", string followerIndex = "0")
        {
            return await new BattlesetupReq(this).Send(activeDeckId, followerId, questId, questPhase, followerIndex);
        }
    }
}