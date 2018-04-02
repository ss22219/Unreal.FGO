using Akka.Actor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.TaskService.Actor;
using Unreal.FGO.Helper;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Core;
using Unreal.FGO.Common;
using Akka.Configuration;
using Unreal.FGO.TaskService.TaskExcutor;
using System.IO;
using log4net.Config;
using static AkkaMain;

namespace Unreal.FGO.TaskService.TaskExcutor
{

    public class TaskCheckeActor : TaskExcutorBase
    {
        public TaskCheckeActor()
        {
            ReceiveAsync<object>(async message =>
            {
                while (true)
                {
                    var Db = new Db();
                    var taskIds = Db.userTasks.AsNoTracking().Where(t =>
                    t.enable == true && (t.start_time == null || t.start_time < DateTime.Now)
                    && (t.end_time == null || t.end_time > DateTime.Now) &&
                    (
                        (t.state != 1)
                        || (t.state == 1 && t.expires_time < DateTime.Now)

                    )).Select(t => t.id).ToList();
                    Db.Dispose();
                    foreach (var taskId in taskIds)
                    {
                        Context.ActorOf<TaskExcuteActor>().Tell(taskId);
                    }
                    await Task.Delay(10000);
                }
            });
        }
    }
}
