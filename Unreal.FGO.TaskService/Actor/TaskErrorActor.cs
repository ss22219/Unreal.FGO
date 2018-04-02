using Akka.Actor;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.TaskService.Actor
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
        ITEMUSE = 14,
        DECKSETUP = 15,
    }

    public enum GameError
    {
        NO = 0,
        START_TASK_ERROR = -1024,
        DECKSETUP = -15,
        ITEMUSE = -14,
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
    public class TaskErrorActor : ActorBase
    {
        public TaskErrorActor()
        {
            Receive<TaskErrorInfo>(info =>
           {
               if (info.action == GameAction.START_TASK && info.message.IndexOf("RUNNING") != -1)
                   return;

               using (var db = new Db())
               {
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

                   logger.Error(info.message);
                   logger.Error(info.sourceMessage);
                   logger.Error(info.sourceData);
                   var task = db.userTasks.Find(info.taskId);
                   if (task == null)
                   {
                       Sender.Tell(false);
                       return;
                   }
                   task.error_type = (int)info.error;
                   string nextAction = null;
                   string action = null;
                   if (task.re_excute_count > 0 && task.re_excute_count % 100 == 0)
                   {
                       task.enable = false;
                       nextAction = "重试100次，禁用该计划";
                   }
                   else if (task.re_excute_count > 0 && task.re_excute_count % 10 == 0)
                   {
                       task.start_time = DateTime.Now.AddHours(1);
                       nextAction = "重试10次，一小时后执行";
                   }
                   else
                   {
                       task.start_time = DateTime.Now.AddSeconds(10);
                       nextAction = "，10秒后执行";
                   }
                   task.current_action = (int)info.action;
                   task.re_excute_count++;
                   switch (info.error)
                   {
                       case GameError.NET_ERROR:
                           action = "网络连接失败";
                           break;
                       case GameError.HOME_ERROR:
                           action = "进入首页失败";
                           task.state = 0;
                           task.current_action = (int)GameAction.unknown;
                           break;
                       case GameError.MAINTAIN:
                           action = "服务器正在维护";
                           task.state = (int)TaskState.GAME_ERROR;
                           task.start_time = DateTime.Now.AddHours(3);
                           nextAction = "，3小时后执行";
                           break;
                       case GameError.MEMBER_ERROR:
                           action = "获取游戏初始化信息失败";
                           break;
                       case GameError.API_LOGIN_ERROR:
                           action = "使用帐号登陆B站失败";
                           break;
                       case GameError.API_ZONE_ERROR:
                           action = "提交数据到B站失败";
                           break;
                       case GameError.ITEMUSE:
                           action = "使用苹果失败";
                           break;
                       case GameError.PRESENTLIST_ERROR:
                           action = "获取邮件列表";
                           break;
                       case GameError.PRESENTRECEIVE_ERROR:
                           action = "领取邮件列表";
                           break;
                       case GameError.BATTLESETUP_ERROR:
                           action = "开始战斗失败";
                           break;
                       case GameError.BATTLERESULT_ERROR:
                           action = "结束战斗失败";
                           break;
                       case GameError.TOPLOGIN_ERROR:
                       case GameError.LOGINCENTER_ERROR:
                       case GameError.LOGIN_ERROR:
                           if (info.message.IndexOf("uname") != -1)
                           {
                               task.state = 0;
                               task.current_action = (int)GameAction.unknown;
                           }
                           action = "登录到服务器失败";
                           break;
                       default:
                           task.state = (int)TaskState.GAME_ERROR;
                           break;
                   }
                   db.AddTaskLog(new Repostory.Model.task_log()
                   {
                       action = (int)info.action,
                       create_time = DateTime.Now,
                       message = action + "，" + nextAction
                   });

                   db.SaveChanges();
               }
               Sender.Tell(false);
           });
        }
    }
}
