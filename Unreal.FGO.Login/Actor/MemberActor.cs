using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Console.Helper;
using Unreal.FGO.Core;

namespace Unreal.FGO.Console.Actor
{
    public class MemberActor : ActorBase
    {
        public MemberActor()
        {
            ReceiveAsync<LoginTask>(async task =>
           {
               var serverApi = task.serverApi;
               var member = await serverApi.Member();
               if (member.code == 0)
               {
                   //var version = member.response[0].success.version;
                   //Context.ActorOf<UpdateVersionActor>().Tell(version);
               }
               else
               {
                   await TaskErrorAndBack(task.id, GameAction.MEMBER, member);
                   return;
               }
               Sender.Tell(true);
           });
        }
    }
}
