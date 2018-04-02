using Unreal.FGO.Console.Helper;
using Akka.Actor;
using System;
using System.Threading.Tasks;

namespace Unreal.FGO.Console.Actor
{
    public class TaskExcuteActor : ActorBase
    {
        public TaskExcuteActor()
        {
            ReceiveAsync<int>(async taskId =>
            {
                await Context.ActorOf<UpdateVersionActor>().Ask(string.Empty);
                var db = DbHelper.DB;
                var task = db.GetTaskById(taskId);
                start:
                if (task == null)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "TASK NOT EXISTS \r\n" + "ID: " + taskId);
                    return;
                }
                if (task.enable == false)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "TASK NOT ENABLED \r\n" + "ID: " + taskId);
                    return;
                }
                if (task.end_time != null && task.end_time.Value < DateTime.Now)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "TASK IS END！\r\n" + "ID: " + taskId);
                    return;
                }
                if (task.state == (int)TaskState.RUNNING)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "TASK IS RUNNING！\r\n" + "ID: " + taskId);
                    return;
                }
                var role = db.GetUserRoleById(task.user_role_id);
                if (role == null)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "ROLE NOT EXISTS \r\n" + "ID: " + taskId + ", ROLE_ID: " + task.user_role_id);
                    return;
                }
                var device = db.GetDeviceById(role.device_id);
                if (device == null)
                {
                    await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "DEVICE NOT EXISTS！\r\n" + "ID: " + taskId + ", ROLE_ID: " + task.user_role_id + ",DEVICE_ID: " + role.device_id);
                    return;
                }
                var data = db.GetRoleDataByRoleId(role.id);
                bool success = true;
                switch (task.action)
                {
                    case "Login":
                        task.state = (int)TaskState.RUNNING;
                        db.Update(task);
                        success = await Context.ActorOf<LoginActor>().Ask<bool>(new LoginTask()
                        {
                            id = taskId,
                            data = data,
                            device = device,
                            role = role
                        });
                        task = db.GetTaskById(taskId);
                        if (!success)
                        {
                            if (task.enable && task.start_time != null && task.start_time > DateTime.Now)
                            {
                                await Task.Delay(task.start_time.Value - DateTime.Now);
                                goto start;
                            }
                        }
                        else
                        {
                            task.re_excute_count = 0;
                            task.error_type = 0;
                            task.state = 0;
                            task.last_update_time = DateTime.Now;
                            db.Update(task);
                        }
                        Sender.Tell(true);
                        break;

                    case "Battle":
                        task.state = (int)TaskState.RUNNING;
                        db.Update(task);
                        success = await Context.ActorOf<BattleActor>().Ask<bool>(new BattleTask()
                        {
                            id = taskId,
                            data = data,
                            device = device,
                            role = role,
                            questIds = task.quest_ids.Split(','),
                            useitem = task.useitem
                        });
                        task = db.GetTaskById(taskId);
                        if (!success)
                        {
                            if (task.enable && task.start_time != null && task.start_time > DateTime.Now)
                            {
                                await Task.Delay(task.start_time.Value - DateTime.Now);
                                goto start;
                            }
                        }
                        else
                        {
                            task.re_excute_count = 0;
                            task.error_type = 0;
                            task.last_update_time = DateTime.Now;
                            task.state = 0;
                            db.Update(task);
                            goto start;
                        }
                        break;
                    default:
                        await TaskErrorAndBack(taskId, GameAction.START_TASK, null, "ID: " + taskId + " UNKNOWN ACTION！");
                        return;
                }

            });
        }
    }
}