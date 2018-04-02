using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core;
using Akka.Actor;
using Newtonsoft.Json;

namespace Unreal.FGO.TaskService.Actor
{
    public class HomeActor : ActorBase
    {
        public HomeActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                if (task.serverApi == null)
                    task.serverApi = InitServerApi(task);
                var home = await task.serverApi.Home();
                if (home.code != 0)
                {
                    if (home.code == 88)
                    {
                        await Context.ActorOf<ClearTokenActor>().Ask(task.role.id);
                        await taskResultActor.Ask<bool>(new TaskResult()
                        {
                            nextAction = GameAction.unknown,
                            enable = false
                        });
                        await TaskErrorBack(task.id, GameAction.HOME, home, "Token过期，帐号在其他地方登陆");
                        Sender.Tell(false);
                        return;
                    }
                    await TaskErrorBack(task.id, GameAction.HOME, home);
                    return;
                }
                else
                {
                    if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userQuest != null)
                        task.roleData.quest_info = JsonConvert.SerializeObject(home.cache.replaced.userQuest);
                    if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userSvt != null)
                        task.roleData.svt_info = JsonConvert.SerializeObject(home.cache.replaced.userSvt);
                    if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userGame != null)
                        task.roleData.user_game = JsonConvert.SerializeObject(home.cache.replaced.userGame[0]);
                    if (task.serverApi.userItem != null)
                        task.roleData.user_item = JsonConvert.SerializeObject(task.serverApi.userItem);
                    await SaveActionData(task, GameAction.HOME);
                    if (home.cache.replaced != null && home.cache.replaced.userPresentBox != null)
                    {
                        var listRes = await task.serverApi.Presentlist();
                        if (listRes.code == 0)
                        {
                            var receiveRes = await task.serverApi.Presentreceive(listRes.cache.replaced.userPresentBox.Select(b => b.presentId).ToArray());
                            if (receiveRes.code == 0)
                            {
                                if (receiveRes.cache != null && receiveRes.cache.updated != null && receiveRes.cache.updated.userGame != null)
                                    task.roleData.user_game = JsonConvert.SerializeObject(receiveRes.cache.updated.userGame[0]);

                                if (task.serverApi.userItem != null)
                                    task.roleData.user_item = JsonConvert.SerializeObject(task.serverApi.userItem);
                                await SaveActionData(task, GameAction.PRESENTRECEIVE);
                            }
                            else
                            {
                                await TaskErrorBack(task.id, GameAction.PRESENTRECEIVE, receiveRes);
                            }
                        }
                        else
                        {
                            await TaskErrorBack(task.id, GameAction.PRESENTLIST, listRes);
                        }
                    }
                }
                Log(task.id, GameAction.HOME, "获取首页信息成功");
                Sender.Tell(true);
            });
        }
    }
}
