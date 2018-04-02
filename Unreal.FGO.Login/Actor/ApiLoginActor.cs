using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Console.Helper;
using Unreal.FGO.Core;

namespace Unreal.FGO.Console.Actor
{
    public class ApiLoginActor : ActorBase
    {
        public ApiLoginActor()
        {
            ReceiveAsync<LoginTask>(async (task) =>
            {
                var serverApi = task.serverApi;
                var role = task.role;
                var data = task.data;
                var flag = serverApi.PlatfromInfos.ContainsKey("access_token") && serverApi.PlatfromInfos.ContainsKey("expires");
                flag = flag && long.Parse(serverApi.PlatfromInfos["expires"]) > serverApi.Timetamp;

                if (!flag)
                {
                    var rsa = await serverApi.ApiClientRsa();
                    if (rsa.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.API_LOGIN, rsa, "API_RSA_ERROR");
                        return;
                    }
                    var access_key = serverApi.PlatfromInfos.ContainsKey("access_token") ? serverApi.PlatfromInfos["access_token"] : null;
                    await Task.Delay(1000);
                    var login = await serverApi.ApiClientLogin(role.username, role.password, rsa.rsa_key, rsa.hash, access_key);
                    if (login.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.API_LOGIN, rsa, "API_LOGIN_ERROR");
                        return;
                    }
                    var userInfo = await serverApi.ApiClientUserInfo(login.access_key);
                    if (userInfo.code != 0)
                    {
                        await TaskErrorAndBack(task.id, GameAction.API_LOGIN, rsa, "API_USER_INFO_ERROR");
                        return;
                    }
                    data.access_token = serverApi.PlatfromInfos["access_token"];
                    data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
                    data.bilibili_id = serverApi.PlatfromInfos["uid"];
                    data.nickname = serverApi.PlatfromInfos["uname"];
                    data.s_face = serverApi.PlatfromInfos["s_face"];
                    data.face = serverApi.PlatfromInfos["face"];
                    Context.ActorOf<SaveRoleDataActor>().Tell(data);
                }
                Sender.Tell(true);
            });
        }
    }
}

