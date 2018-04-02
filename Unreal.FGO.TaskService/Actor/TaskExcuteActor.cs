using Unreal.FGO.Helper;
using Akka.Actor;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.TaskService.Actor
{
    public enum ExcuteType
    {
        Normal = 0,
        Expires = 1,
        ErrorReStart = 2
    }

    public class TaskExcuteActor : ActorBase
    {
        private user_task task;
        private user_role role;
        private role_data roleData;
        private device device;
        private Dictionary<string, string> taskDatas = null;
        ExcuteType excuteType = ExcuteType.Normal;

        public TaskExcuteActor()
        {
            ReceiveAsync<int>(async taskId =>
            {
                //await Context.ActorOf<UpdateVersionActor>().Ask(string.Empty);
                Log(taskId, GameAction.START_TASK, "正在准备执行任务");
                using (var db = new Db())
                    task = db.GetTaskById(taskId);
                if (task == null)
                {
                    await TaskError(taskId, GameAction.START_TASK, null, "TASK NOT EXISTS \r\n" + "ID: " + taskId);
                    return;
                }
                if (task.enable == false)
                {
                    Log(taskId, GameAction.START_TASK, "任务被禁用，请编辑任务后启动");
                    await TaskError(taskId, GameAction.START_TASK, null, "TASK NOT ENABLED \r\n" + "ID: " + taskId);
                    return;
                }
                if (task.end_time != null && task.end_time.Value < DateTime.Now)
                {
                    Log(taskId, GameAction.START_TASK, "任务已经到期");
                    await TaskError(taskId, GameAction.START_TASK, null, "TASK IS END！\r\n" + "ID: " + taskId);
                    return;
                }
                if (task.state == (int)TaskState.RUNNING && task.expires_time > DateTime.Now)
                {
                    Log(taskId, GameAction.START_TASK, "检查到任务正在进行中，等待结果");
                    await TaskError(taskId, GameAction.START_TASK, null, "TASK IS RUNNING！\r\n" + "ID: " + taskId);
                    return;
                }
                using (var db = new Db())
                    role = db.GetUserRoleById(task.user_role_id);
                if (role == null)
                {
                    Log(taskId, GameAction.START_TASK, "执行帐号不存在，任务无法执行");
                    await TaskError(taskId, GameAction.START_TASK, null, "ROLE NOT EXISTS \r\n" + "ID: " + taskId + ", ROLE_ID: " + task.user_role_id);
                    return;
                }
                using (var db = new Db())
                    device = db.GetDeviceById(role.device_id);
                if (device == null)
                {
                    Log(taskId, GameAction.START_TASK, "执行帐号设备信息不存在，任务无法执行");
                    await TaskError(taskId, GameAction.START_TASK, null, "DEVICE NOT EXISTS！\r\n" + "ID: " + taskId + ", ROLE_ID: " + task.user_role_id + ",DEVICE_ID: " + role.device_id);
                    return;
                }
                if (task.state != 0 && task.expires_time < DateTime.Now)
                    excuteType = ExcuteType.Expires;
                else if (task.state == (int)TaskState.NET_ERROR || task.state == (int)TaskState.GAME_ERROR)
                    excuteType = ExcuteType.ErrorReStart;
                if (excuteType == ExcuteType.Expires)
                    Log(taskId, GameAction.START_TASK, "检测任务执行超时，恢复任务中");
                else if (excuteType == ExcuteType.ErrorReStart)
                    Log(taskId, GameAction.START_TASK, "检测任务执行失败，恢复任务中");

                using (var db = new Db())
                {
                    roleData = db.GetRoleDataByRoleId(role.id);
                    if (roleData == null)
                    {
                        roleData = new role_data()
                        {
                            role_id = role.id,
                            last_login = new DateTime(1990,1,1)
                        };
                        db.Add(roleData);
                    }

                    var taskDataList = db.GetTaskDataByTaskId(task.id);
                    var dataDic = new Dictionary<string, string>();
                    foreach (var item in taskDataList)
                    {
                        dataDic[item.name] = item.value;
                    }
                    taskDatas = dataDic;
                    db.Database.ExecuteSqlCommand("update user_task set state=@p0 where id=@p1", (int)TaskState.RUNNING, taskId);
                }
                if (task.current_action == 1024)
                    task.current_action = 0;
                switch (task.action)
                {
                    case "Login":
                        await ExcuteLoginTask();
                        break;

                    case "Battle":
                        await ExcuteBattleTask();
                        break;
                    default:
                        Log(task.id, GameAction.START_TASK, "未知的计划类型，禁用该计划");
                        using (var db = new Db())
                            db.Database.ExecuteSqlCommand("update user_task set state=0,enable=0,current_action=0 where id=@p1", taskId);
                        await TaskError(taskId, GameAction.START_TASK, null, "ID: " + taskId + " UNKNOWN ACTION！");
                        return;
                }
                Sender.Tell(true);
            });
        }

        private async Task ExcuteBattleTask()
        {
            if (task.quest_ids == null)
            {
                Log(task.id, GameAction.START_TASK, "战斗任务没有副本信息，禁用该计划");
                await TaskError(task.id, GameAction.START_TASK, null, "ID: " + task.id + " NOT QUEST！");

                using (var db = new Db())
                    db.Database.ExecuteSqlCommand("update user_task set state=0,enable=0,current_action=0 where id=@p0", task.id);
                return;
            }
            var battleTask = new BattleTask()
            {
                id = task.id,
                roleData = roleData,
                device = device,
                role = role,
                task_data = taskDatas,
                excuteType = excuteType,
                deckId = task.deckid,
                action = (GameAction)task.current_action,
                followerId = task.follower_id,
                useitem = task.useitem,
                questIds = task.quest_ids.Split(',')
            };
            await Context.ActorOf<BattleTaskActor>().Ask(battleTask);
        }

        private async Task ExcuteLoginTask()
        {
            if (DateTime.Now.Day == task.last_update_time.Day)
            {
                Log(task.id, GameAction.START_TASK, "检查到登录计划已经成功执行，将在24小时后执行");
                using (var db = new Db())
                    db.Database.ExecuteSqlCommand(
                    "update user_task set state=0,start_time=@p0 where id=@p1",
                    task.last_update_time.AddDays(1),
                    task.id);
                return;
            }
            var loginTask = new LoginTask()
            {
                id = task.id,
                roleData = roleData,
                device = device,
                role = role,
                task_data = taskDatas,
                action = (GameAction)task.current_action,
                excuteType = excuteType
            };
            if (await Context.ActorOf<LoginTaskActor>().Ask<bool>(loginTask))
            {
                Log(task.id, GameAction.START_TASK, "登录计划已经成功执行，将在24小时后执行");
                using (var db = new Db())
                    db.Database.ExecuteSqlCommand(
                    "update user_task set state=0,start_time=@p0 where id=@p1",
                    DateTime.Now.AddDays(1), task.id);
            }
        }
    }
}