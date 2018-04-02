using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.TaskService.Actor
{
    public class LoginTask
    {
        internal ExcuteType excuteType;

        public LoginTask()
        {
            task_data = new Dictionary<string, string>();
        }
        public int id { get; set; }
        public GameAction action { get; set; }
        public user_role role { get; set; }
        public device device { get; set; }
        public role_data roleData { get; set; }
        public Dictionary<string, string> task_data { get; set; }
        public ServerApi serverApi { get; set; }
    }

    public class LoginTaskActor : ActorBase
    {
        public LoginTaskActor()
        {
            ReceiveAsync<LoginTask>(async (task) =>
            {
                Log(task.id, GameAction.LOGIN, "准备登陆");
                var device = task.device;
                var role = task.role;
                var data = task.roleData;

                if (task.serverApi == null)
                    task.serverApi = InitServerApi(task);
                var serverApi = task.serverApi;

                if (data == null)
                {
                    task.roleData = data = new role_data()
                    {
                        role_id = role.id
                    };
                }
                task.serverApi = serverApi;
                if
                (
                    await Context.ActorOf<MemberActor>().Ask<bool>(task)
                     && await Context.ActorOf<ApiLoginActor>().Ask<bool>(task)
                     && await Context.ActorOf<LoginToMemberCenterActor>().Ask<bool>(task)
                     && await Context.ActorOf<HomeActor>().Ask<bool>(task)
                )
                {
                    Sender.Tell(true);
                }
                Sender.Tell(false);
            });
        }
    }
}
