using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory;
using Akka.Actor;
namespace Unreal.FGO.TaskService.Actor
{
    public class RoleChapteredActor : ActorBase
    {
        public RoleChapteredActor()
        {
            Receive<int>(roleId =>
            {
                using (var db = new Db())
                {
                    db.Database.ExecuteSqlCommand("update role_data set chaptered=1 where role_id=@p0", roleId);
                }
                Sender.Tell(true);
            });
        }
    }
}
