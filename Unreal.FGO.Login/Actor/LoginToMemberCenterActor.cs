using System.Threading.Tasks;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;
using Akka.Actor;

namespace Unreal.FGO.Console.Actor
{
    public class LoginToMemberCenterActor : ActorBase
    {
        public LoginToMemberCenterActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                var serverApi = task.serverApi;
                if (!serverApi.PlatfromInfos.ContainsKey("usk") || !serverApi.PlatfromInfos.ContainsKey("logintomembercenter"))
                {
                    var loginCenter = await serverApi.LoginToMemberCenter();
                    if (loginCenter.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.LOGINCENTER, loginCenter, "LoginToMemberCenter Error");
                        return;
                    }

                    var login = await serverApi.Login();
                    if (loginCenter.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.LOGIN, login, "LOGIN Error");
                        return;
                    }

                    var zone = await serverApi.ApiClientNotifyZone();
                    if (zone.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.API_ZONE, zone, "API_ZONE Error");
                        return;
                    }

                    var toplogin = await serverApi.Toplogin();
                    if (toplogin.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.TOPLOGIN, toplogin, "TOP_LOGIN Error");
                        return;
                    }
                    serverApi.PlatfromInfos["logintomembercenter"] = "1";
                    var data = new role_data()
                    {
                        role_id = task.data.role_id,
                        id = task.data.id,
                        usk = serverApi.PlatfromInfos["usk"],
                        rguid = serverApi.PlatfromInfos["rguid"],
                        game_user_id = serverApi.PlatfromInfos["userId"]
                    };
                    Context.ActorOf<SaveRoleDataActor>().Tell(data);
                }
                Sender.Tell(true);
            });
        }
    }
}