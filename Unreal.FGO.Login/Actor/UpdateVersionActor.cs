using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Console.Helper;

namespace Unreal.FGO.Console.Actor
{
    public class UpdateVersionActor : ActorBase
    {
        public UpdateVersionActor()
        {
            Receive<string>(version =>
           {
               var infos = DbHelper.DB.systemInfos.ToList();
               AppInfo.version = infos.Where( i=>i.name == "version").First().value;
               AppInfo.dataVer = infos.Where(i => i.name == "dataVer").First().value;
               AppInfo.dateVer = infos.Where(i => i.name == "dateVer").First().value;
               AppInfo.appVer = infos.Where(i => i.name == "appVer").First().value;
               Context.Sender.Tell(true);
           });
        }
    }
}
