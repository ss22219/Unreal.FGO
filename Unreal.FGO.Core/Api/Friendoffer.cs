using System.Threading.Tasks;
using Unreal.FGO.Core.Api;
using System.Collections.Generic;

namespace Unreal.FGO.Core.Api
{
    public class FriendofferSuccess
    {
        public string message { get; set; }
        public string status { get; set; }
        public string title { get; set; }
    }
    public class FriendofferResponse
    {
        public string resCode { get; set; }
        public FriendofferSuccess success { get; set; }
        public fail fail { get; set; }
        public string nid { get; set; }
        public string usk { get; set; }
    }
    public class FriendofferTblfriend
    {
        public int userId { get; set; }
        public string friendId { get; set; }
        public int updatedAt { get; set; }
        public int createdAt { get; set; }
        public int status { get; set; }
    }
    public class FriendofferUpdated
    {
        public List<FriendofferTblfriend> tblFriend { get; set; }
    }
    public class FriendofferReplaced
    {
    }
    public class FriendofferCache
    {
        public FriendofferUpdated updated { get; set; }
        public FriendofferReplaced replaced { get; set; }
        public int serverTime { get; set; }
    }
    public class FriendofferRes : BaseResponse
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
        public List<FriendofferResponse> response { get; set; }
        public FriendofferCache cache { get; set; }
    }
    
    public class FriendofferReq: BaseRequest<FriendofferRes>
    {
        public FriendofferReq(ServerApi api) : base(api)
        {
        }
    
        public override async Task<FriendofferRes> Send(params string[] args)
        {
                 PlatfromInfos["ac"] = "action";
                 PlatfromInfos["key"] = "friendoffer";
            var url = "/rongame_beta/rgfate/60_1001/ac.php";
            string getParam = null;
            getParam = getPlatfromInfo(new string[]{"_userId"});
            if (!string.IsNullOrEmpty(getParam))
                url += "?" + getParam;
            Dictionary<string,string> postParam = null;
            postParam = getPlatfromInfoDic(new string[]{"ac","key","deviceid","os","ptype","usk","umk","rgsid","rkchannel","targetUserId","userId","appVer","dateVer","lastAccessTime","try","developmentAuthCode","userAgent","dataVer"});
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
        public async Task<FriendofferRes> Friendoffer()
        {
            return await new FriendofferReq(this).Send();
        }
    }
}