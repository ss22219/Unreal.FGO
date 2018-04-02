using System.Threading.Tasks;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;
using Akka.Actor;
using Unreal.FGO.Helper;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Unreal.FGO.TaskService.Actor
{
    public class LoginToMemberCenterActor : ActorBase
    {
        public LoginToMemberCenterActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                try
                {

                    if (task.serverApi == null)
                        task.serverApi = InitServerApi(task);
                    var serverApi = task.serverApi;
                    if (!serverApi.PlatfromInfos.ContainsKey("uname"))
                    {
                        Log(task.id, GameAction.LOGINCENTER, "登陆失败，缓存中没有b站帐号信息");
                        await Context.ActorOf<ClearTokenActor>().Ask(task.role.id);
                        await TaskErrorBack(task.id, GameAction.LOGINCENTER, null, "LOGINCENTER uname NULL");
                        return;
                    }
                    if (!serverApi.PlatfromInfos.ContainsKey("usk") || !serverApi.PlatfromInfos.ContainsKey("logintomembercenter"))
                    {
                        var loginCenter = await serverApi.LoginToMemberCenter();
                        if (loginCenter.code != 0)
                        {
                            await TaskErrorBack(task.id, GameAction.LOGINCENTER, loginCenter, "LoginToMemberCenter Error");
                            return;
                        }
                        if (AppInfo.version != loginCenter.response[0].success.dataVer)
                        {
                            await updateVersionActor.Ask(new VersionInfo()
                            {
                                version = loginCenter.response[0].success.dataVer,
                                dateVer = loginCenter.response[0].success.dateVer
                            });
                        }
                        var login = await serverApi.Login();
                        if (login.code != 0)
                        {
                            await TaskErrorBack(task.id, GameAction.LOGIN, login, "LOGIN Error");
                            return;
                        }

                        var zone = await serverApi.ApiClientNotifyZone();
                        if (zone.code != 0)
                        {
                            logger.Warn("ApiClientNotifyZone:" + zone.message);
                            //await TaskErrorBack(task.id, GameAction.API_ZONE, zone, "API_ZONE Error");
                            //return;
                        }
                        
                        var type = login.response[0].success.type == "signup" ? "signuptop" : "toplogin";
                        var toplogin = await serverApi.Toplogin(type);
                        if (toplogin.code != 0)
                        {
                            await TaskErrorBack(task.id, GameAction.TOPLOGIN, toplogin, "TOP_LOGIN Error");
                            return;
                        }
                        serverApi.PlatfromInfos["logintomembercenter"] = "1";
                        var data = task.roleData;
                        data.usk = serverApi.PlatfromInfos["usk"];
                        data.rguid = serverApi.PlatfromInfos["rguid"];
                        data.game_user_id = serverApi.PlatfromInfos["userId"];
                        if (toplogin.cache.replaced.userQuest != null)
                            data.quest_info = JsonConvert.SerializeObject(toplogin.cache.replaced.userQuest);

                        if (toplogin.cache.replaced.userDeck != null)
                            data.deck_info = JsonConvert.SerializeObject(toplogin.cache.replaced.userDeck);
                        else if (toplogin.cache.updated.userDeck != null)
                            data.deck_info = JsonConvert.SerializeObject(toplogin.cache.updated.userDeck);

                        if (toplogin.cache.replaced.userSvt != null)
                            data.svt_info = JsonConvert.SerializeObject(toplogin.cache.replaced.userSvt);

                        if(toplogin.cache.replaced.userGame != null && toplogin.cache.replaced.userGame.Count > 0)
                            data.user_game = JsonConvert.SerializeObject(toplogin.cache.replaced.userGame[0]);
                        await SaveActionData(task, GameAction.LOGINCENTER);
                        Log(task.id, GameAction.LOGINCENTER, "成功登陆到服务器");
                    }
                    else
                    {
                        Log(task.id, GameAction.LOGINCENTER, "使用游戏缓存数据登陆");
                    }
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