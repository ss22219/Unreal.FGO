using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Unreal.FGO.Core.Api
{
    public class GachadrawGachainfos
    {
        public string isNew { get; set; }
        public string userSvtId { get; set; }
        public string type { get; set; }
        public string objectId { get; set; }
        public string num { get; set; }
        public string limitCount { get; set; }
    }
    public class GachadrawSuccess
    {
        public List<GachadrawGachainfos> gachaInfos { get; set; }
    }
    public class GachadrawResponse
    {
        public string resCode { get; set; }
        public GachadrawSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public string isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class GachadrawUsergame
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
        public string carryOverActPostring { get; set; }
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
        public string weekFriendPostring { get; set; }
        public string chargeRewardRecord { get; set; }
        public string firstgoldtime { get; set; }
        public string loginresettime { get; set; }
        public string last_ac { get; set; }
        public string last_ac_time { get; set; }
        public string appVer { get; set; }
        public string tstring01 { get; set; }
        public string tstring02 { get; set; }
        public string tstring03 { get; set; }
        public string tstring04 { get; set; }
        public string tstring05 { get; set; }
        public string tvar01 { get; set; }
        public string tvar02 { get; set; }
        public string tvar03 { get; set; }
        public string tvar04 { get; set; }
        public string tvar05 { get; set; }
        public string stone { get; set; }
        public string friendKeep { get; set; }
        public string svtKeep { get; set; }
        public string svtEquipKeep { get; set; }
    }
    public class GachadrawUsersvt
    {
        public string userId { get; set; }
        public string svtId { get; set; }
        public string limitCount { get; set; }
        public string lv { get; set; }
        public string dispLimitCount { get; set; }
        public string imageLimitCount { get; set; }
        public string commandCardLimitCount { get; set; }
        public string iconLimitCount { get; set; }
        public string exp { get; set; }
        public string skillLv1 { get; set; }
        public string skillLv2 { get; set; }
        public string skillLv3 { get; set; }
        public string status { get; set; }
        public string condVal { get; set; }
        public string treasureDeviceLv1 { get; set; }
        public string treasureDeviceLv2 { get; set; }
        public string treasureDeviceLv3 { get; set; }
        public string equipTargetId1 { get; set; }
        public string adjustHp { get; set; }
        public string adjustAtk { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
        public string hp { get; set; }
        public string atk { get; set; }
        public string treasureDeviceId1 { get; set; }
        public string treasureDeviceId2 { get; set; }
        public string treasureDeviceId3 { get; set; }
        public string skillId1 { get; set; }
        public string selectTreasureDeviceIdx { get; set; }
        public string isLock { get; set; }
        public string id { get; set; }
    }
    public class GachadrawUsersvtcollection
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
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
    }
    public class GachadrawUsergacha
    {
        public string userId { get; set; }
        public string gachaId { get; set; }
        public string num { get; set; }
        public string freeDrawAt { get; set; }
        public long updatedAt { get; set; }
        public long createdAt { get; set; }
    }
    public class GachadrawUpdated
    {
        public List<GachadrawUsergame> userGame { get; set; }
        public List<ToploginUsersvt> userSvt { get; set; }
        public List<GachadrawUsersvtcollection> userSvtCollection { get; set; }
        public List<GachadrawUsergacha> userGacha { get; set; }
    }
    public class GachadrawCache
    {
        public GachadrawUpdated updated { get; set; }
    }
    public class GachadrawRes : BaseResponse
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
        public List<GachadrawResponse> response { get; set; }
        public GachadrawCache cache { get; set; }
    }

    public class GachadrawReq : BaseRequest<GachadrawRes>
    {
        public GachadrawReq(ServerApi api) : base(api)
        {
        }

        public override async Task<GachadrawRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "gachadraw";
            PlatfromInfos["gachaId"] = args[0];
            PlatfromInfos["shopIdIndex"] = args[1];
            PlatfromInfos["num"] = args[2];
            PlatfromInfos["ticketItemId"] = args[3];
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "gachaId", "num", "ticketItemId", "shopIdIndex", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                serverApi.UpdateUserSvt(response.cache.updated.userSvt);
            }
            return response;

        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public List<ToploginUsersvt> userSvt { get; set; }

        public async Task<GachadrawRes> Gachadraw(string gachaId, string shopIdIndex, string num, string ticketItemId = "0")
        {
            return await new GachadrawReq(this).Send(gachaId, shopIdIndex, num, ticketItemId);
        }

        public void UpdateUserSvt(List<ToploginUsersvt> userSvt)
        {
            if (this.userSvt == null)
                this.userSvt = userSvt;
            else
            {
                foreach (var svt in userSvt)
                {
                    if (!this.userSvt.Any(u => u.id == svt.id))
                    {
                        this.userSvt.Add(svt);
                    }
                    else
                    {
                        var i = this.userSvt.FindIndex(u => u.id == svt.id);
                        this.userSvt[i] = svt;
                    }
                }
            }
        }
    }
}