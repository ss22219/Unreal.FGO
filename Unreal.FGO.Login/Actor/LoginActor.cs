using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Console.Helper;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.Console.Actor
{
    public class LoginTask
    {
        public int id { get; set; }
        public GameAction action { get; set; }
        public user_role role { get; set; }
        public device device { get; set; }
        public role_data data { get; set; }
        public ServerApi serverApi { get; set; }
    }

    public class LoginActor : ActorBase
    {
        private ServerApi serverApi;
        private device device;
        private role_data data;
        private user_role role;

        public void InitServerApi()
        {
            serverApi = new ServerApi(device.platform_type == 1);
            serverApi.PlatfromInfos["version"] = AppInfo.version;
            serverApi.PlatfromInfos["dataVer"] = AppInfo.dataVer;
            serverApi.PlatfromInfos["dateVer"] = AppInfo.dateVer;
            serverApi.PlatfromInfos["deviceid"] = device.deviceid;
            serverApi.PlatfromInfos["dp"] = device.dp;
            serverApi.PlatfromInfos["idfa"] = device.idfa;
            serverApi.PlatfromInfos["model"] = device.model;
            serverApi.PlatfromInfos["os"] = device.os;
            serverApi.PlatfromInfos["pf_ver"] = device.pf_ver;
            serverApi.PlatfromInfos["platform_type"] = device.platform_type.ToString();
            serverApi.PlatfromInfos["ptype"] = device.ptype;
            serverApi.PlatfromInfos["udid"] = device.udid;

            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.access_token))
                {
                    serverApi.PlatfromInfos["access_token"] = data.access_token;
                    serverApi.PlatfromInfos["access_key"] = data.access_token;
                    serverApi.PlatfromInfos["expires"] = data.access_token_expires.ToString();
                }
                if (!string.IsNullOrEmpty(data.usk))
                {
                    serverApi.PlatfromInfos["usk"] = data.usk;
                    serverApi.PlatfromInfos["rgusk"] = data.usk;
                }
                if (!string.IsNullOrEmpty(data.rguid))
                    serverApi.PlatfromInfos["rguid"] = data.rguid;
                if (!string.IsNullOrEmpty(data.game_user_id))
                {
                    serverApi.PlatfromInfos["rguid"] = data.rguid;
                    serverApi.PlatfromInfos["_userId"] = data.game_user_id;
                    serverApi.PlatfromInfos["userId"] = data.game_user_id;
                    serverApi.PlatfromInfos["role_id"] = data.game_user_id;
                }
                if (!string.IsNullOrEmpty(data.nickname))
                {
                    serverApi.PlatfromInfos["nickname"] = data.nickname;
                    serverApi.PlatfromInfos["uname"] = data.nickname;
                }
                if(!string.IsNullOrEmpty(data.bilibili_id))
                    serverApi.PlatfromInfos["uid"] = data.bilibili_id;
            }
            ServerApiManager.SetServerApi(role.id, serverApi);
        }

        public LoginActor()
        {
            ReceiveAsync<LoginTask>(async (task) =>
            {
                device = task.device;
                role = task.role;
                data = task.data;
                serverApi = task.serverApi == null ? ServerApiManager.GetByRoleId(role.id) : task.serverApi;

                if (serverApi == null)
                {
                    InitServerApi();
                    task.serverApi = serverApi;
                }

                if (data == null)
                {
                    task.data = data = new role_data()
                    {
                        role_id = role.id
                    };
                }
                task.serverApi = serverApi;
                Sender.Tell
                (
                    await Context.ActorOf<MemberActor>().Ask<bool>(task)
                     && await Context.ActorOf<ApiLoginActor>().Ask<bool>(task)
                     && await Context.ActorOf<LoginToMemberCenterActor>().Ask<bool>(task)
                );
            });
        }
    }
}
