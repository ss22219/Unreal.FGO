using Akka.Actor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Console.Helper;

namespace Unreal.FGO.Console.Actor
{
    public enum TaskState
    {
        CONFIG_ERROR = -3,
        GAME_ERROR = -2,
        NET_ERROR = -1,
        STOP = 0,
        RUNNING = 1,
        ASYNC = 2
    }

    public enum GameAction
    {
        START_TASK = 1024,
        unknown = 0,
        PRESENTRECEIVE = 13,
        PRESENTLIST = 12,
        BATTLERESULT = 11,
        BATTLESETUP = 10,
        HOME = 9,
        TOPLOGIN = 8,
        LOGIN = 7,
        LOGINCENTER = 6,
        API_ZONE = 5,
        API_LOGIN = 4,
        MEMBER = 3,
    }

    public enum GameError
    {
        NO = 0,
        START_TASK_ERROR = -1024,
        PRESENTRECEIVE_ERROR = -13,
        PRESENTLIST_ERROR = -12,
        BATTLERESULT_ERROR = -11,
        BATTLESETUP_ERROR = -10,
        HOME_ERROR = -9,
        TOPLOGIN_ERROR = -8,
        LOGIN_ERROR = -7,
        LOGINCENTER_ERROR = -6,
        API_ZONE_ERROR = -5,
        API_LOGIN_ERROR = -4,
        MEMBER_ERROR = -3,
        NET_ERROR = -1,
        MAINTAIN = 1
    }
    public class TaskErrorActor : ReceiveActor
    {
        public TaskErrorActor()
        {
            Receive<TaskErrorInfo>(info =>
            {
                var color = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(JsonConvert.SerializeObject(info, Formatting.Indented));
                System.Console.ForegroundColor = color;
                var db = DbHelper.DB;
                db.taskError.Add(new Repostory.Model.task_error()
                {
                    task_id = info.taskId,
                    error = (int)info.error,
                    action = (int)info.action,
                    message = info.message,
                    create_time = DateTime.Now,
                    source_code = info.sourceCode,
                    source_data = info.sourceData,
                    source_message = info.sourceMessage
                });

                db.SaveChanges();
                var task = db.GetTaskById(info.taskId);
                task.error_type = (int)info.error;
                switch (info.error)
                {
                    case GameError.NET_ERROR:
                        task.state = (int)TaskState.NET_ERROR;
                        if (task.re_excute_count > 0 && task.re_excute_count % 100 == 0)
                            task.enable = false;
                        if (task.re_excute_count > 0 && task.re_excute_count % 5 == 0)
                            task.start_time = DateTime.Now.AddHours(1);
                        else
                            task.start_time = DateTime.Now.AddSeconds(10);
                        task.current_action = (int)info.action;
                        task.re_excute_count++;
                        break;

                    case GameError.HOME_ERROR:
                        task.state = (int)TaskState.GAME_ERROR;
                        if (task.re_excute_count > 0 && task.re_excute_count % 100 == 0)
                            task.enable = false;
                        if (task.re_excute_count > 0 && task.re_excute_count % 5 == 0)
                            task.start_time = DateTime.Now.AddHours(1);
                        else
                            task.start_time = DateTime.Now.AddSeconds(5);
                        task.current_action = (int)info.action;
                        task.re_excute_count++;
                        break;
                    case GameError.MAINTAIN:
                        task.state = (int)TaskState.GAME_ERROR;
                        task.start_time = DateTime.Now.AddHours(3);
                        break;
                    case GameError.START_TASK_ERROR:
                        task.state = (int)TaskState.CONFIG_ERROR;
                        task.enable = false;
                        Sender.Tell(true);
                        break;
                    default:
                        task.state = (int)TaskState.GAME_ERROR;
                        break;
                }
                db.SaveChanges();
                Sender.Tell(false);
            });
        }
    }
}
