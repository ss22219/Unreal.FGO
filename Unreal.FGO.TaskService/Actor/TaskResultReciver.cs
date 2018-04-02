using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.TaskService.Actor
{
    public class TaskResult
    {
        public int id { get; set; }
        public TaskState? state { get; set; }
        public DateTime? nextStartTime { get; set; }
        public GameAction? nextAction { get; set; }
        public bool? enable { get; set; }
    }

    public class TaskResultReciver : ReceiveActor
    {
        public TaskResultReciver()
        {
            Receive<TaskResult>(result =>
           {
               using (var db = new Db())
               {
                   var task = db.userTasks.Find(result.id);
                   if (task != null)
                   {
                       task.start_time = result.nextStartTime != null ? result.nextStartTime : task.start_time;
                       task.enable = result.enable != null ? result.enable.Value : task.enable;
                       task.current_action = result.nextAction == null ? task.current_action : (int)result.nextAction;
                       task.state = result.state == null ? task.state : (int)result.state;
                       task.last_update_time = DateTime.Now;
                       task.error_type = 0;
                       task.re_excute_count = 0;
                       db.SaveChanges();
                   }
               }
               Sender.Tell(true);
           });
        }
    }
}
