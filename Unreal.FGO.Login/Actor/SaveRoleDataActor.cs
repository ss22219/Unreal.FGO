using System;
using Unreal.FGO.Console.Helper;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.Console.Actor
{
    public class SaveActionDataTask
    {
        public int taskId { get; set; }
        public GameAction action { get; set; }
        public DateTime expiresTime { get; set; }
        public ServerApi serverApi { get; set; }
        public string battleId { get; internal set; }
    }

    public class SaveRoleDataActor : ActorBase
    {
        public SaveRoleDataActor()
        {
            Receive<SaveActionDataTask>(taskInfo =>
            {
                var serverApi = taskInfo.serverApi;
                var db = DbHelper.DB;
                var task = db.GetTaskById(taskInfo.taskId);
                if (task == null)
                    return;
                task.current_action = (int)(GameAction)taskInfo.action;
                task.expiresTime = taskInfo.expiresTime;
                task.battlId = taskInfo.battleId;
                db.Update(task);

                var data = db.GetRoleDataByRoleId(task.user_role_id);
                if (data == null)
                    return;
                data.usk = serverApi.PlatfromInfos["usk"];
                data.rguid = serverApi.PlatfromInfos["rguid"];
                data.game_user_id = serverApi.PlatfromInfos["userId"];
                data.access_token = serverApi.PlatfromInfos["access_token"];
                data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
                data.bilibili_id = serverApi.PlatfromInfos["uid"];
                db.Update(data);
            });

            Receive<role_data>(data =>
            {
                var db = DbHelper.DB;
                var db_data = db.GetRoleDataByRoleId(data.role_id);
                if (db_data == null)
                    db_data = data;
                else
                {
                    if (!string.IsNullOrEmpty(data.nickname))
                    {
                        db_data.face = data.face;
                        db_data.s_face = data.s_face;
                        db_data.nickname = data.nickname;
                    }
                    if (!string.IsNullOrEmpty(data.access_token))
                    {
                        db_data.access_token = data.access_token;
                        db_data.access_token_expires = data.access_token_expires;
                        db_data.bilibili_id = data.bilibili_id;
                    }
                    db_data.game_user_id = string.IsNullOrEmpty(data.game_user_id) ? db_data.game_user_id : data.game_user_id;
                    db_data.rguid = string.IsNullOrEmpty(data.rguid) ? db_data.rguid : data.rguid;
                    db_data.usk = string.IsNullOrEmpty(data.usk) ? db_data.usk : data.usk;
                }
                if (db_data.id == 0)
                    db.roleData.Add(db_data);
                db.SaveChanges();
            });

            Receive<LoginTask>(task =>
            {
                var serverApi = task.serverApi;
                var db = DbHelper.DB;
                var data = db.GetRoleDataByRoleId(task.role.id);
                if (data == null)
                    return;
                data.usk = serverApi.PlatfromInfos["usk"];
                data.rguid = serverApi.PlatfromInfos["rguid"];
                data.game_user_id = serverApi.PlatfromInfos["userId"];
                data.access_token = serverApi.PlatfromInfos["access_token"];
                data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
                data.bilibili_id = serverApi.PlatfromInfos["uid"];
                db.SaveChanges();
            });
        }
    }
}