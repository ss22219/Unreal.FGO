using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;
using System;

namespace Unreal.FGO.Core.Api
{
    public class ItemuseSuccess
    {
    }
    public class ItemuseResponse
    {
        public string resCode { get; set; }
        public ItemuseSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class ItemuseUseritem
    {
        public string userId { get; set; }
        public string itemId { get; set; }
        public int num { get; set; }
        public string updatedAt { get; set; }
        public string createdAt { get; set; }
    }
    public class ItemuseUsergame
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
        public int actRecoverAt { get; set; }
        public int carryOverActPoint { get; set; }
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
    public class ItemuseUpdated
    {
        public List<HomeUseritem> userItem { get; set; }
        public List<HomeUsergame> userGame { get; set; }
    }
    public class ItemuseCache
    {
        public ItemuseUpdated updated { get; set; }
        public int serverTime { get; set; }
        public ItemuseSuccess replaced { get; set; }
    }
    public class ItemuseRes : BaseResponse
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
        public List<ItemuseResponse> response { get; set; }
        public ItemuseCache cache { get; set; }
    }

    public class ItemuseReq : BaseRequest<ItemuseRes>
    {
        public ItemuseReq(ServerApi api) : base(api)
        {
        }

        public override async Task<ItemuseRes> Send(params string[] args)
        {
            PlatfromInfos["ac"] = "action";
            PlatfromInfos["key"] = "itemuse";
            replaceInfos = new Dictionary<string, string>() {
                { "itemId",args[0] },
                { "num",args[1]}
            };

            var url = "/rongame_beta//rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[] { "_userId" });
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string, string> postParam = null;
            postParam = getPlatfromInfoDic(new string[] { "ac", "key", "deviceid", "os", "ptype", "usk", "umk", "rgsid", "rkchannel", "userId", "appVer", "dateVer", "lastAccessTime", "try", "developmentAuthCode", "userAgent", "itemId", "num", "questId", "dataVer" });
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
        public List<HomeUseritem> userItem { get; set; }

        public async Task<ItemuseRes> Itemuse(string itemId, string num)
        {
            return await new ItemuseReq(this).Send(itemId, num);
        }
        public void UpdateItem(List<HomeUseritem> userItem)
        {
            if (this.userItem == null)
            {
                this.userItem = userItem;
            }
            else if (userItem != null && userItem.Count > 0)
            {
                var count = this.userItem.Count;
                var count2 = userItem.Count;
                for (int i = 0; i < count2; i++)
                {
                    var find = false;
                    for (var j = 0; j < count; j++)
                    {
                        if (this.userItem[j].itemId == userItem[i].itemId)
                        {
                            this.userItem[j] = userItem[i];
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                        this.userItem.Add(userItem[i]);
                }

            }
        }
    }
}