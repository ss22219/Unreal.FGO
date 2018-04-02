using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Repostory;
using Unreal.FGO.Helper;
using Unreal.FGO.Core.Api;
using Newtonsoft.Json;
using log4net;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public abstract class TaskExcutorBase : ReceiveActor
    {
        protected ILog logger;
        public TaskExcutorBase()
        {
            logger = LogManager.GetLogger(this.GetType());
            InitAppInfo();
        }

        private static void InitAppInfo()
        {
            if (!string.IsNullOrEmpty(AppInfo.appVer))
                return;
            Db Db = new Db();
            foreach (var item in Db.systemInfos.AsNoTracking().ToList())
            {
                if (item.name == "version")
                    AppInfo.version = item.value;
                else if (item.name == "dateVer")
                    AppInfo.dateVer = item.value;
                else if (item.name == "dataVer")
                    AppInfo.dataVer = item.value;
                else if (item.name == "appVer")
                    AppInfo.appVer = item.value;
            }
            Db.Dispose();
        }

        protected device getDevice(int id)
        {
            var db = new Db();
            var dv = db.GetDeviceById(id);
            db.Dispose();
            return dv;
        }
        protected role_data getRoleData(int roleId)
        {
            using (var db = new Db())
            {
                var roleData = db.GetRoleDataByRoleId(roleId);
                if (roleData == null)
                {
                    roleData = new role_data()
                    {
                        role_id = roleId,
                        last_login = DateTime.Now
                    };
                    if (roleData.role_id != 0)
                    {
                        db.roleData.Add(roleData);
                        db.SaveChanges();
                    }
                }
                return roleData;
            }
        }

        protected ServerApi InitServerApi(user_role role, device device, role_data data)
        {
            var serverApi = new ServerApi(device.platform_type == 1);
            serverApi.PlatfromInfos["version"] = AppInfo.version;
            serverApi.PlatfromInfos["dataVer"] = AppInfo.dataVer;
            serverApi.PlatfromInfos["dateVer"] = AppInfo.dateVer;
            serverApi.PlatfromInfos["appVer"] = AppInfo.appVer;
            serverApi.PlatfromInfos["ver"] = AppInfo.appVer;
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
                serverApi.PlatfromInfos["cookie"] = data.cookie;
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
                if (!string.IsNullOrEmpty(data.bilibili_id))
                    serverApi.PlatfromInfos["uid"] = data.bilibili_id;

                if (data.quest_info != null)
                    serverApi.userQuest = JsonConvert.DeserializeObject<List<HomeUserquest>>(data.quest_info);

                if (data.svt_info != null)
                    serverApi.userSvt = JsonConvert.DeserializeObject<List<ToploginUsersvt>>(data.svt_info);

                if (data.deck_info != null)
                    serverApi.userDeck = JsonConvert.DeserializeObject<List<ToploginUserdeck>>(data.deck_info);

                if (data.user_game != null)
                    serverApi.userGame = JsonConvert.DeserializeObject<HomeUsergame>(data.user_game);
            }
            return serverApi;
        }
    }
}
