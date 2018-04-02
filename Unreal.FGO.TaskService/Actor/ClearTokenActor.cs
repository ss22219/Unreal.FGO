using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Repostory;
using Akka.Actor;
namespace Unreal.FGO.TaskService.Actor
{
    public class ClearTokenActor : ActorBase
    {
        public ClearTokenActor()
        {
            Receive<int>(roleId =>
            {
                using (var db = new Db())
                {
                    db.Database.ExecuteSqlCommand("update role_data set access_token='',usk='' where role_id=@p0", roleId);
                }
                Sender.Tell(true);
            });
        }
    }
}
