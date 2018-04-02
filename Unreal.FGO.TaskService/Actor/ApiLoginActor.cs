using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Core;

namespace Unreal.FGO.TaskService.Actor
{
    public class ApiLoginActor : ActorBase
    {
        public ApiLoginActor()
        {
            ReceiveAsync<LoginTask>(async (task) =>
            {
                try
                {

                    if (task.serverApi == null)
                        task.serverApi = InitServerApi(task);
                    var serverApi = task.serverApi;
                    var role = task.role;
                    var data = task.roleData;
                    var flag = serverApi.PlatfromInfos.ContainsKey("access_token") && serverApi.PlatfromInfos.ContainsKey("expires");
                    flag = flag && long.Parse(serverApi.PlatfromInfos["expires"]) > serverApi.Timetamp;

                    if (!flag)
                    {
                        var rsa = await serverApi.ApiClientRsa();
                        if (rsa.code != 0)
                        {
                            await TaskError(task.id, GameAction.API_LOGIN, rsa, "API_RSA_ERROR");
                            return;
                        }
                        var access_key = serverApi.PlatfromInfos.ContainsKey("access_token") ? serverApi.PlatfromInfos["access_token"] : null;
                        await Task.Delay(1000);
                        var login = await serverApi.ApiClientLogin(role.username, role.password, rsa.rsa_key, rsa.hash, access_key);
                        if (login.code != 0)
                        {
                            await TaskError(task.id, GameAction.API_LOGIN, rsa, "API_LOGIN_ERROR");
                            return;
                        }
                        var userInfo = await serverApi.ApiClientUserInfo(login.access_key);
                        if (userInfo.code != 0)
                        {
                            await TaskError(task.id, GameAction.API_LOGIN, rsa, "API_USER_INFO_ERROR");
                            return;
                        }
                        data.access_token = serverApi.PlatfromInfos["access_token"];
                        data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
                        data.bilibili_id = serverApi.PlatfromInfos["uid"];
                        data.nickname = serverApi.PlatfromInfos["uname"];
                        data.s_face = serverApi.PlatfromInfos["s_face"];
                        data.face = serverApi.PlatfromInfos["face"];
                        await SaveActionData(task, GameAction.API_LOGIN, data);
                        Log(task.id, GameAction.API_LOGIN, "登陆到B站成功");
                    }
                    else
                        Log(task.id, GameAction.API_LOGIN, "使用B站缓存数据登陆");
                    Sender.Tell(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Sender.Tell(false);
                }
            });
        }
    }
}

