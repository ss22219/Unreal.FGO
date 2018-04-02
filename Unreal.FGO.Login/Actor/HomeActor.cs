using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core;
using Akka.Actor;
namespace Unreal.FGO.Console.Actor
{
    public class HomeActor : ActorBase
    {
        public HomeActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                var home = await task.serverApi.Home();
                if (home.code != 0)
                {
                    if (home.code == 88)
                    {
                        task.serverApi.PlatfromInfos.Remove("usk");
                        Context.ActorOf<SaveRoleDataActor>().Tell(task);
                    }
                    await TaskErrorAndBack(task.id, GameAction.HOME, home);
                    return;
                }
                else
                    Context.ActorOf<SaveRoleDataActor>().Tell(task);
                Sender.Tell(true);
            });
        }
    }
}
