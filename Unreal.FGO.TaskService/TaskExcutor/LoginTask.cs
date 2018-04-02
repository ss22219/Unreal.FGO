using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Repostory;
using Unreal.FGO.TaskService.Actor;
using Unreal.FGO.Common.ActorResult;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class Login : TaskExcutorBase
    {
        public Login()
        {
            ReceiveAsync<LoginParam>(async param =>
            {
                try
                {
                    var result = new LoginResult()
                    {
                        code = 0,
                        message = "success"
                    };
                    var device = param.device;
                    var role = param.role;
                    var roleData = getRoleData(role.id);
                    if (param.device == null) {

                        result.code = -12;
                        result.message = "设备不存在";
                        Sender.Tell(result);
                        return;
                    }
                    var serverApi = InitServerApi(param.role, param.device, roleData);

                    logger.Info("登陆:" + role.username);

                    var task = new LoginTask()
                    {
                        device = device,
                        role = role,
                        serverApi = serverApi,
                        roleData = roleData
                    };

                    if (!await Context.ActorOf<MemberActor>().Ask<bool>(task))
                    {
                        result.code = -1;
                        result.message = "获取游戏版本失败";
                        Sender.Tell(result);
                        return;
                    }

                    if (!await Context.ActorOf<ApiLoginActor>().Ask<bool>(task))
                    {
                        result.code = -2;
                        result.message = "登陆B站失败";
                        Sender.Tell(result);
                        return;
                    }

                    if (!await Context.ActorOf<LoginToMemberCenterActor>().Ask<bool>(task))
                    {
                        result.code = -3;
                        result.message = "登陆游戏失败";
                        Sender.Tell(result);
                        return;
                    }

                    if (param.home && !await Context.ActorOf<HomeActor>().Ask<bool>(task))
                    {
                        result.code = -3;
                        result.message = "登陆游戏主页失败";
                        Sender.Tell(result);
                        return;
                    }
                    result.usk = serverApi.PlatfromInfos["usk"];
                    Sender.Tell(result);
                }
                catch (Exception ex)
                {
                    Sender.Tell(new LoginResult()
                    {
                        code = -500,
                        message = ex.ToString()
                    });
                }
            });
        }
    }
}
