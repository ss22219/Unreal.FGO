using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class ProfileeditnameSuccess
    {
        public int isCreateRole { get; set; }
        public string name { get; set; }
        public string createTime { get; set; }
    }
    public class ProfileeditnameResponse
    {
        public string resCode { get; set; }
        public ProfileeditnameSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
        public int isBattleLive { get; set; }
        public List<string> questIds { get; set; }
    }
    public class ProfileeditnameUsergame
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
        public int genderType { get; set; }
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
        public int friendKeep { get; set; }
        public int svtKeep { get; set; }
        public int svtEquipKeep { get; set; }
    }
    public class ProfileeditnameUpdated
    {
        public List<HomeUsergame> userGame { get; set; }
    }
    public class ProfileeditnameReplaced
    {
    }
    public class ProfileeditnameCache
    {
        public ProfileeditnameUpdated updated { get; set; }
        public ProfileeditnameReplaced replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class ProfileeditnameRes : BaseResponse
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
        public List<ProfileeditnameResponse> response { get; set; }
        public ProfileeditnameCache cache { get; set; }
    }

    public class ProfileeditnameReq : BaseRequest<ProfileeditnameRes>
    {
        public ProfileeditnameReq(ServerApi api) : base(api)
        {
        }

        public override async Task<ProfileeditnameRes> Send(params string[] args)
        {
            PlatfromInfos["name"] = args[0];
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "profileeditname";
            PlatfromInfos["genderType"] = "2";
            PlatfromInfos["isCreateRole"] = "1";
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "name", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "genderType", "isCreateRole", "dataVer" });
            var response = await Post(url, postParam);
            if (response.code == 0)
            {
                PlatfromInfos["usk"] = response.response[0].usk;
                if (response.cache.updated.userGame != null)
                    serverApi.userGame = response.cache.updated.userGame[0];
            }
            return response;

        }
    }


}

namespace Unreal.FGO.Core
{
    public partial class ServerApi
    {
        public async Task<ProfileeditnameRes> Profileeditname(string username)
        {
            return await new ProfileeditnameReq(this).Send(username);
        }
    }
}