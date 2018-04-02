using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Core.Api;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Core;
using System.Collections.Concurrent;
using log4net;
using Newtonsoft.Json;

namespace Unreal.FGO.TaskService.Actor
{
    public class TaskErrorInfo
    {
        public GameError error { get; set; }
        public GameAction action { get; set; }
        public string message { get; set; }
        public int taskId { get; set; }
        public string sourceCode { get; set; }
        public string sourceMessage { get; set; }
        public string sourceData { get; set; }
    }

    public class ActorBase : ReceiveActor
    {
        protected ILog logger;
        public void Log(int taskId, GameAction action, string message)
        {
            logger.Info(message);
            if (taskId == 0)
                return;
            using (var db = new Db())
            {
                db.AddTaskLog(new task_log()
                {
                    action = (int)action,
                    message = message,
                    create_time = DateTime.Now,
                    task_id = taskId,
                });
                var count = db.taskLog.Where(l => l.task_id == taskId).Count();
                if (count > 100)
                    db.taskLog.RemoveRange(db.taskLog.Where(l => l.task_id == taskId).OrderBy(l => l.create_time).Take(count - 100));
            }
        }
        public ActorBase()
        {
            logger = LogManager.GetLogger(this.GetType());
            if (saveDataActor == null)
                saveDataActor = Context.ActorOf<SaveDataActor>("SaveDataActor");
            if (taskErrorActor == null)
                taskErrorActor = Context.ActorOf<TaskErrorActor>("TaskErrorActor");
            if (taskResultActor == null)
                taskResultActor = Context.ActorOf<TaskResultReciver>("TaskResultActor");
            if (updateVersionActor == null)
                updateVersionActor = Context.ActorOf<UpdateVersionActor>("UpdateVersionActor");
        }

        protected static IActorRef saveDataActor;
        protected static IActorRef taskErrorActor;
        protected static IActorRef taskResultActor;
        protected static IActorRef updateVersionActor;

        public static ServerApi InitServerApi(LoginTask task)
        {
            var device = task.device;
            var data = task.roleData;
            var role = task.role;

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
        protected async Task SaveActionData(LoginTask task, GameAction action, DateTime? expireTime = null, Dictionary<string, string> taskData = null)
        {
            var saveActionDataTask = new SaveActionDataTask()
            {
                taskId = task.id,
                action = action,
                expiresTime = expireTime,
                roleData = GetRoleData(task.serverApi, task.role.id, task.role.user_id, task.roleData),
                taskData = taskData == null ? task.task_data : taskData,

            };
            await saveDataActor.Ask(saveActionDataTask);
        }
        protected async Task SaveActionData(LoginTask task, GameAction action, role_data data, DateTime? expireTime = null, Dictionary<string, string> taskData = null)
        {
            var saveActionDataTask = new SaveActionDataTask()
            {
                taskId = task.id,
                action = action,
                expiresTime = expireTime,
                roleData = data ?? GetRoleData(task.serverApi, task.role.id, task.role.user_id),
                taskData = taskData == null ? task.task_data : taskData,

            };
            await saveDataActor.Ask(saveActionDataTask);
        }

        protected async Task SaveActionData(int taskId, GameAction action, ServerApi serverApi, user_role role, DateTime? expireTime = null, Dictionary<string, string> taskData = null)
        {
            var saveActionDataTask = new SaveActionDataTask()
            {
                taskId = taskId,
                action = action,
                expiresTime = expireTime,
                roleData = GetRoleData(serverApi, role.id, role.user_id),
                taskData = taskData
            };
            await saveDataActor.Ask(saveActionDataTask);
        }

        protected role_data GetRoleData(ServerApi serverApi, int roleId, int userId = 0, role_data ndata = null)
        {
            var data = new role_data()
            {
                role_id = roleId
            };
            data.usk = serverApi.PlatfromInfos.ContainsKey("usk") ? serverApi.PlatfromInfos["usk"] : null;
            data.rguid = serverApi.PlatfromInfos["rguid"];
            data.game_user_id = serverApi.PlatfromInfos["userId"];
            data.access_token = serverApi.PlatfromInfos["access_token"];
            data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
            data.bilibili_id = serverApi.PlatfromInfos["uid"];
            if (serverApi.PlatfromInfos.ContainsKey("cookie"))
                data.cookie = serverApi.PlatfromInfos["cookie"];
            if (ndata.quest_info != null)
                data.quest_info = ndata.quest_info;
            if (ndata.deck_info != null)
                data.deck_info = ndata.deck_info;
            if (ndata.svt_info != null)
                data.svt_info = ndata.svt_info;
            if (ndata.user_game != null)
                data.user_game = ndata.user_game;
            if (ndata.user_item != null)
                data.user_item = ndata.user_item;
            if (ndata.cookie != null)
                data.cookie = ndata.cookie;
            if (ndata.battle_id != null)
                data.battle_id = ndata.battle_id;
            return data;
        }

        protected async Task TaskError(TaskErrorInfo info, BaseResponse response = null, string message = null)
        {
            if (message != null)
                info.message = message;
            if (response != null)
            {
                info.sourceCode = response.code.ToString();
                info.sourceData = response.RequestMessage;
                info.sourceMessage = response.message;
                if (response.message != null && response.message.IndexOf("维护") != -1)
                {
                    info.error = GameError.MAINTAIN;
                }
                else if (response.code == 99)
                {
                    info.error = GameError.NET_ERROR;
                }
            }
            await taskErrorActor.Ask(info);
        }

        protected async Task TaskErrorBack(int taskId, GameAction action, BaseResponse response = null, string message = null)
        {
            if (message == null)
                message = action.ToString() + " Error";
            var info = new TaskErrorInfo()
            {
                action = action,
                error = (GameError)(-(int)action),
                taskId = taskId
            };
            await TaskError(info, response, message);
            Sender.Tell(false);
        }

        protected async Task TaskError(int taskId, GameAction action, BaseResponse response = null, string message = null)
        {
            if (message == null)
                message = action.ToString() + " Error";
            var info = new TaskErrorInfo()
            {
                action = action,
                error = (GameError)(-(int)action),
                taskId = taskId
            };
            await TaskError(info, response, message);
        }
    }
}
