using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory.Model;
using Akka.Actor;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Repostory;
using Unreal.FGO.TaskService.Actor;
using Newtonsoft.Json;
using Unreal.FGO.Core;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class InitRole : TaskExcutorBase
    {

        public InitRole()
        {
            ReceiveAsync<InitRoleParam>(async (param) =>
            {
                logger.Info("InitRole Starting..");
                var role = param.role;
                var res = new LoginResult();
                try
                {
                    var db = new Db();
                    var device = db.GetDeviceById(role.device_id);
                    db.Dispose();
                    var login = Context.ActorOf<Login>();

                    var loginRes = await login.Ask<LoginResult>(new LoginParam()
                    {
                        role = role,
                        device = device
                    });

                    if (loginRes.code != 0)
                    {
                        Sender.Tell(loginRes);
                        return;
                    }

                    var data = getRoleData(role.id);
                    var serverApi = InitServerApi(role, device, data);
                    if (role.inited)
                    {
                        logger.Info(role.username + "已经初始成功");
                        await Home(serverApi, data, role);
                        Sender.Tell(res);
                        return;
                    }
                    var battleSetup = Context.ActorOf<BattleSetupActor>();
                    var battleResult = Context.ActorOf<BattleResultActor>();
                    BattleTask task = new BattleTask()
                    {
                        serverApi = serverApi,
                        task_data = new Dictionary<string, string>() {
                        { "deckId","" },
                        { "followerId","0" },
                        { "questId","1000000" },
                        { "questPhase","1" },
                    },
                        role = role,
                        device = device,
                        followerId = "0",
                        deckId = "",
                        roleData = data
                    };
                    if (serverApi.userGame != null && serverApi.userGame.tutorial1 == "8198")
                    {
                        logger.Info(role.username + "已经初始成功");
                        await Home(serverApi, data, role);
                        db = new Db();
                        role = db.userRoles.Find(role.id);
                        role.last_update_time = DateTime.Now;
                        role.inited = true;
                        db.SaveChanges();
                        Sender.Tell(res);
                    }
                    logger.Info(role.username + "初始开始");
                    var questIds = new string[] { "1000000", "1000001", "1000002" };
                    foreach (var questId in questIds)
                    {
                        if (serverApi.userGame != null
                                && serverApi.userQuest.Any(u => u.questId == "1000000")
                                && serverApi.userGame.name == serverApi.userGame.userId
                           )
                        {

                            logger.Info("设置名字");
                            await SetName(task, res, param.name);
                            if (res.code != 0)
                            {
                                Sender.Tell(res);
                                return;
                            }
                            logger.Info("名字设置成功");
                        }
                        if (new QuestChecker() { userQuest = serverApi.userQuest, TimetampSecond = serverApi.TimetampSecond }.CheckQuestClear(questId))
                            continue;
                        await Battle(task, questId, res, battleSetup, battleResult);
                        if (res.code != 0)
                        {
                            Sender.Tell(res);
                            return;
                        }
                    }

                    var tutorialResult = await serverApi.Tutorialprogress("1");
                    var gachaRes = await serverApi.Gachadraw("101", "2", "10");
                    //Gachadraw
                    var tutorialSetResult = await serverApi.Tutorialset("101");
                    tutorialResult = await serverApi.Tutorialprogress("2");


                    //var gachaRes = await serverApi.Decksetup(serverApi.userDeck);
                    //
                    tutorialResult = await serverApi.Tutorialprogress("3");

                    await Battle(task, "1000003", res, battleSetup, battleResult);
                    if (res.code != 0)
                    {
                        Sender.Tell(res);
                        return;
                    }
                    await Home(serverApi, data, role);
                    db = new Db();
                    role = db.userRoles.Find(role.id);
                    role.last_update_time = DateTime.Now;
                    role.inited = true;
                    db.SaveChanges();

                    Sender.Tell(res);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    res.message = ex.Message;
                    res.code = -500;
                    Sender.Tell(res);
                }
            });
        }

        public async Task Home(ServerApi serverApi, role_data data, user_role role)
        {
            logger.Info("登陆首页领取礼物");
            try
            {
                var home = await serverApi.Home();
                if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userQuest != null)
                    data.quest_info = JsonConvert.SerializeObject(home.cache.replaced.userQuest);
                if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userSvt != null)
                    data.svt_info = JsonConvert.SerializeObject(home.cache.replaced.userSvt);
                if (home.cache != null && home.cache.replaced != null && home.cache.replaced.userGame != null)
                    data.user_game = JsonConvert.SerializeObject(home.cache.replaced.userGame[0]);

                //检查章节完成情况
                if (!string.IsNullOrEmpty(data.quest_info) && !role.chaptered && role.id != 0)
                {
                    logger.Info(role.username + " 检查章节完成情况");
                    var questChecker = new QuestChecker()
                    {
                        TimetampSecond = serverApi.TimetampSecond,
                        userQuest = JsonConvert.DeserializeObject<List<HomeUserquest>>(data.quest_info)
                    };
                    var hasQuest = false;
                    var questList = questChecker.GetUserQuestList();
                    foreach (var quest in questList)
                    {
                        if (quest.type != "1")
                            continue;
                        var spot = AssetManage.Database.mstSpot.FirstOrDefault(s => s.id == quest.spotId);
                        if (spot == null)
                            continue;
                        var war = AssetManage.Database.mstWar.FirstOrDefault(w => w.id == spot.warId && w.status == "0");
                        if (war == null)
                            continue;
                        hasQuest = true;
                        break;
                    }
                    if (!hasQuest)
                    {
                        await Context.ActorOf<RoleChapteredActor>().Ask<bool>(role.id);
                        logger.Info(role.username + " 章节清除完成");
                    }

                }

                var list = await serverApi.Presentlist();
                var receiveRes = await serverApi.Presentreceive(list.cache.replaced.userPresentBox.Select(b => b.presentId).ToArray());
                if (receiveRes.cache != null && receiveRes.cache.updated != null && receiveRes.cache.updated.userGame != null)
                    data.user_game = JsonConvert.SerializeObject(receiveRes.cache.updated.userGame[0]);

                data.usk = serverApi.PlatfromInfos.ContainsKey("usk") ? serverApi.PlatfromInfos["usk"] : null;
                data.rguid = serverApi.PlatfromInfos["rguid"];
                data.game_user_id = serverApi.PlatfromInfos["userId"];
                data.access_token = serverApi.PlatfromInfos["access_token"];
                data.access_token_expires = long.Parse(serverApi.PlatfromInfos["expires"]);
                data.bilibili_id = serverApi.PlatfromInfos["uid"];
                await SaveActionData(data, GameAction.HOME);
                logger.Info("id:" + data.role_id + " name:" + home.cache.replaced.userGame[0].name + " stone:" + home.cache.replaced.userGame[0].stone);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private Task SaveActionData(role_data data, GameAction action)
        {
            var saveActionDataTask = new SaveActionDataTask()
            {
                action = action,
                roleData = data,
            };
            return Context.ActorOf<SaveDataActor>().Ask(saveActionDataTask);
        }

        public async Task SetName(BattleTask task, LoginResult res, string name = null)
        {
            var serverApi = task.serverApi;
            var role = task.role;
            if (res.code != 0)
            {
                return;
            }
            if (name == null)
                name = role.username.Length > 6 ? role.username.Substring(0, 6) + new Random().Next(1, 100) : role.username;
            else
                name = name.Length > 6 ? name.Substring(0, 6) : name;
            var setNameRes = await serverApi.Profileeditname(name);
            if (setNameRes.code != 0)
            {
                res.code = -500;
                res.message = setNameRes.message;
                return;
            }
            var createRoleRes = await serverApi.ApiClientCreateRole(name);
            if (createRoleRes.code != 0)
            {
                res.code = -500;
                res.message = createRoleRes.message;
                return;
            }
            return;
        }

        public async Task Battle(BattleTask task, string questId, LoginResult res, IActorRef battleSetup, IActorRef battleResult)
        {
            var serverApi = task.serverApi;
            var data = task.roleData;
            task.task_data["questId"] = questId;
            var decks = JsonConvert.DeserializeObject<List<Unreal.FGO.Core.Api.ToploginUserdeck>>(data.deck_info);
            task.task_data["deckId"] = decks.First().id.ToString();
            task.deckId = task.task_data["deckId"];

            var questChecker = new QuestChecker()
            {
                TimetampSecond = serverApi.TimetampSecond,
                userQuest = serverApi.userQuest == null ? new List<Core.Api.HomeUserquest>() : serverApi.userQuest
            };
            var questList = questChecker.GetUserQuestList();
            var quest = questList.Where(q => questId == q.id).FirstOrDefault();
            if (quest == null)
            {
                return;
            }

            var count = AssetManage.Database.mstQuestPhase.Where(p => p.questId == quest.id).Count();
            var userQuest = serverApi.userQuest.FirstOrDefault(u => u.questId == quest.id);
            if (count > 0 && userQuest != null && userQuest.questPhase < count)
                task.task_data["questPhase"] = (++userQuest.questPhase).ToString();
            else
                task.task_data["questPhase"] = "1";
            task.task_data["questId"] = quest.id;

            if (res.code != 0)
            {
                Sender.Tell(res);
                return;
            }
            var sleepTime = await battleSetup.Ask<TimeSpan>(task);
            if (sleepTime.TotalMilliseconds == 0)
            {
                res.code = -1;
                res.message = "战斗开始失败:" + questId;
                Sender.Tell(res);
                return;
            }
            logger.Info("战斗开始:" + questId);
            logger.Info("战斗开始，等待" + sleepTime.Minutes + "分" + sleepTime.Seconds + "秒");
            await Task.Delay(sleepTime);
            if (await battleResult.Ask<bool>(task))
            {
                logger.Info("战斗结束");
            }
        }
    }
}
