using Unreal.FGO.Helper;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;
using System;
using System.Collections.Generic;
using Akka.Actor;
using System.Linq;
using Unreal.FGO.Repostory;
using Newtonsoft.Json;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.TaskService.Actor
{
    public class SaveActionDataTask
    {
        public Dictionary<string, string> taskData { get; set; }
        public role_data roleData { get; set; }
        public GameAction action { get; set; }
        public DateTime? expiresTime { get; set; }
        public int taskId { get; set; }
    }
    public class SaveDataActor : ActorBase
    {
        public SaveDataActor()
        {
            Receive<SaveActionDataTask>(taskInfo =>
            {
                if (taskInfo.roleData.role_id == 0)
                {
                    Sender.Tell(false);
                    return;
                }
                lock (typeof(SaveDataActor))
                {
                    using (var db = new Db())
                    {
                        if (taskInfo.action == GameAction.LOGINCENTER)
                        {
                            int stone = 0;
                            if (taskInfo.roleData.user_game != null)
                                stone = JsonConvert.DeserializeObject<HomeUsergame>(taskInfo.roleData.user_game).stone;
                            db.Database.ExecuteSqlCommand("update role_data set usk=@p0,rguid=@p1,game_user_id=@p2,quest_info=@p4,svt_info=@p5,deck_info=@p6,user_game=@p7,last_login=@p8,stone=@p9,user_item=@p10 where role_id=@p3",
                                taskInfo.roleData.usk,
                                taskInfo.roleData.rguid,
                                taskInfo.roleData.game_user_id,
                                taskInfo.roleData.role_id,
                                taskInfo.roleData.quest_info,
                                taskInfo.roleData.svt_info,
                                taskInfo.roleData.deck_info,
                                taskInfo.roleData.user_game,
                                DateTime.Now,
                                stone,
                               taskInfo.roleData.user_item
                             );
                        }
                        else if (taskInfo.action == GameAction.HOME || taskInfo.action == GameAction.PRESENTRECEIVE)
                        {
                            int stone = 0;
                            if (taskInfo.roleData.user_game != null)
                                stone = JsonConvert.DeserializeObject<HomeUsergame>(taskInfo.roleData.user_game).stone;
                            db.Database.ExecuteSqlCommand("update role_data set usk=@p0,quest_info=@p2,svt_info=@p3,user_game=@p4,stone=@p5,user_item=@p6 where role_id=@p1",
                                taskInfo.roleData.usk,
                                taskInfo.roleData.role_id,
                                taskInfo.roleData.quest_info,
                                taskInfo.roleData.svt_info,
                                taskInfo.roleData.user_game,
                                stone,
                               taskInfo.roleData.user_item
                             );
                            db.Database.ExecuteSqlCommand("update user_role set last_update_time=@p0 where id=@p1",
                                DateTime.Now,
                                taskInfo.roleData.role_id
                             );
                        }
                        else if (taskInfo.action == GameAction.API_LOGIN)
                        {
                            db.Database.ExecuteSqlCommand("update role_data set access_token=@p0,access_token_expires=@p1,bilibili_id=@p2,nickname=@p3,s_face=@p4,face=@p5 where role_id=@p6",
                                taskInfo.roleData.access_token,
                                taskInfo.roleData.access_token_expires,
                                taskInfo.roleData.bilibili_id,
                                taskInfo.roleData.nickname,
                                taskInfo.roleData.s_face,
                                taskInfo.roleData.face,
                                taskInfo.roleData.role_id
                             );
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(taskInfo.roleData.usk))
                                db.Database.ExecuteSqlCommand("update role_data set usk=@p0 where role_id=@p1",
                                    taskInfo.roleData.usk,
                                    taskInfo.roleData.role_id
                                );
                        }
                        if (taskInfo.action == GameAction.BATTLESETUP)
                        {
                            if (!string.IsNullOrEmpty(taskInfo.roleData.quest_info))
                                db.Database.ExecuteSqlCommand("update role_data set quest_info=@p0,battle_id=@p2 where role_id=@p1",
                                    taskInfo.roleData.quest_info,
                                    taskInfo.roleData.role_id,
                                    taskInfo.roleData.battle_id
                                );
                        }
                        else if (taskInfo.action == GameAction.BATTLERESULT)
                        {
                            if (!string.IsNullOrEmpty(taskInfo.roleData.quest_info))
                                db.Database.ExecuteSqlCommand("update role_data set quest_info=@p0,battle_id='' where role_id=@p1",
                                    taskInfo.roleData.quest_info,
                                    taskInfo.roleData.role_id
                                );
                        }
                        if (!string.IsNullOrEmpty(taskInfo.roleData.cookie))
                            db.Database.ExecuteSqlCommand("update role_data set cookie=@p0 where role_id=@p1",
                                taskInfo.roleData.cookie,
                                taskInfo.roleData.role_id
                            );
                        var task = db.GetTaskById(taskInfo.taskId);
                        if (task != null)
                        {
                            if (taskInfo.expiresTime == null)
                                taskInfo.expiresTime = DateTime.Now.AddMinutes(5);
                            db.Database.ExecuteSqlCommand("update user_task set error_type=0,re_excute_count=0,current_action=@p0,expires_time=@p1 where id=@p2",
                                (int)taskInfo.action, taskInfo.expiresTime.Value, taskInfo.taskId
                            );
                        }
                        else
                        {
                            Sender.Tell(false);
                            return;
                        }
                        if (taskInfo.taskData == null)
                        {
                            Sender.Tell(true);
                            return;
                        }
                        var taskDatas = db.taskDatas.Where(t => t.task_id == taskInfo.taskId);
                        foreach (var saveItem in taskInfo.taskData)
                        {
                            bool find = false;
                            foreach (var dbItem in taskDatas)
                            {
                                if (dbItem.name == saveItem.Key)
                                {
                                    find = true;
                                    dbItem.value = saveItem.Value;
                                    break;
                                }
                            }
                            if (!find)
                                db.taskDatas.Add(new task_data()
                                {
                                    task_id = taskInfo.taskId,
                                    name = saveItem.Key,
                                    value = saveItem.Value,
                                    user_role_id = taskInfo.roleData == null ? 0 : taskInfo.roleData.role_id
                                });
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Log(taskInfo.taskId, GameAction.unknown, "保存数据失败:" + ex.Message);
                            Sender.Tell(false);
                        }
                        Sender.Tell(true);
                    }
                }
            });
        }
    }
}