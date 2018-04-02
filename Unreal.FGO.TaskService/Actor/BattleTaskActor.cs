using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Core.Api;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.TaskService.Actor
{
    public class BattleTask : LoginTask
    {
        public bool useitem { get; set; }
        public string deckId { get; set; }
        public string followerId { get; set; }
        public string[] questIds { get; set; }
    }

    public class BattleTaskActor : ActorBase
    {
        public static Followerinfo GetFollower(ServerApi serverApi, string followerId = "0")
        {
            if (!string.IsNullOrEmpty(followerId) && followerId != "0")
            {

                var svtIds = followerId.Split(',');
                followerId = null;
                if (serverApi.userFollower != null && serverApi.userFollower.Count > 0 && serverApi.userFollower[0].followerInfo != null)
                {
                    foreach (var svtId in svtIds)
                    {
                        if (string.IsNullOrWhiteSpace(svtId))
                            continue;
                        foreach (var follower in serverApi.userFollower)
                        {
                            if (follower.followerInfo == null)
                                continue;
                            foreach (var info in follower.followerInfo)
                            {
                                if (info.userSvtLeaderHash == null)
                                    continue;
                                for (int i = 0; i < info.userSvtLeaderHash.Count; i++)
                                {
                                    var deck = info.userSvtLeaderHash[i];
                                    if (deck.svtId == svtId)
                                    {
                                        return new Followerinfo() { id = info.userId, index = i, message = "使用好友：" + info.userName + " 从者：" + svtId + " index:" + i };
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (followerId != null)
            {
                return new Followerinfo() { id = followerId, index = 0, message = "使用好友ID：" + followerId };
            }
            else if (serverApi.userFollower != null && serverApi.userFollower.Count > 0 && serverApi.userFollower[0].followerInfo != null && serverApi.userFollower[0].followerInfo.Count > 0)
            {
                followerId = serverApi.userFollower[0].followerInfo.OrderByDescending(f => f.userSvtLeaderHash[0].updatedAt).First().userId;
                return new Followerinfo() { id = followerId, index = 0, message = "没有设置战斗队友, 使用第一个好友战斗" };
            }
            else
            {
                followerId = "0";
                return new Followerinfo() { id = followerId, index = 0, message = "不使用队友战斗支援" };
            }
        }
        protected void Login()
        {
            ReceiveAsync<BattleTask>(
                new Predicate<BattleTask>(task => (int)task.action < (int)(GameAction.HOME) || task.action == GameAction.PRESENTLIST || task.action == GameAction.PRESENTRECEIVE),
                async task =>
                {
                    if (!await Context.ActorOf<LoginTaskActor>().Ask<bool>(task))
                    {
                        Sender.Tell(false);
                    }
                    else
                    {
                        task.excuteType = ExcuteType.Normal;
                        task.action = GameAction.HOME;
                        Self.Tell(task, Sender);
                    }
                });
        }

        protected void BattleSetup()
        {
            ReceiveAsync<BattleTask>(
                new Predicate<BattleTask>(task => task.action == GameAction.HOME || task.action == GameAction.ITEMUSE),
                async task =>
                {
                    if (task.excuteType == ExcuteType.Expires)
                        Log(task.id, GameAction.BATTLERESULT, "正在准备战斗超时重试");
                    else if (task.excuteType == ExcuteType.ErrorReStart)
                        Log(task.id, GameAction.BATTLERESULT, "正在准备战斗错误重试");
                    if (task.serverApi == null && (task.excuteType == ExcuteType.ErrorReStart || task.excuteType == ExcuteType.Expires))
                    {
                        task.action = GameAction.unknown;
                        Self.Tell(task, Sender);
                        return;
                    }
                    if (task.serverApi == null)
                        task.serverApi = InitServerApi(task);
                    var serverApi = task.serverApi;
                    var deckId = task.deckId;

                    if (string.IsNullOrEmpty(task.deckId) && serverApi.userDeck == null && !task.task_data.ContainsKey("deckId"))
                    {
                        Log(task.id, GameAction.BATTLERESULT, "没有战斗编队信息, 战斗失败");
                        await taskResultActor.Ask(new TaskResult()
                        {
                            id = task.id,
                            nextAction = GameAction.unknown,
                            state = TaskState.GAME_ERROR,
                        });
                        await TaskErrorBack(task.id, GameAction.BATTLERESULT, serverApi.LastResponse, "serverApi.userDeck && task_data[deckId] IS NULL");
                        return;
                    }

                    await Context.ActorOf<CheckDeckActor>().Ask<bool>(task);

                    Log(task.id, GameAction.BATTLERESULT, "正在准备战斗数据");
                    if (string.IsNullOrEmpty(task.deckId))
                    {
                        if (serverApi.userDeck != null)
                        {
                            deckId = serverApi.userDeck.First().id.ToString();
                            task.task_data["deckId"] = deckId;
                            Log(task.id, GameAction.BATTLERESULT, "没有设置战斗编队, 使用第一编队战斗");
                        }
                        else
                        {
                            deckId = task.task_data["deckId"];
                            Log(task.id, GameAction.BATTLERESULT, "从保存数据中读取战斗编队");
                        }
                    }
                    else
                    {
                        deckId = serverApi.userDeck[int.Parse(task.deckId)].id.ToString();
                    }
                    var follower = GetFollower(serverApi, task.followerId);
                    Log(task.id, GameAction.BATTLERESULT, follower.message);
                    Log(task.id, GameAction.BATTLESETUP, serverApi.userGame.name + "  lv : " + serverApi.userGame.lv + "    " + "ap : " + serverApi.ApNum);
                    bool hasQuest = false;
                    string questId = null;
                    int ap = serverApi.ApNum;
                    int minAp = 10000000;
                    mstQuest mstQuestData = null;
                    var questList = new QuestChecker()
                    {
                        TimetampSecond = serverApi.TimetampSecond,
                        userQuest = serverApi.userQuest
                    }.GetUserQuestList();
                    var activeQuestList = questList.Where(q => task.questIds.Contains(q.id)).OrderByDescending(q => q.actConsume).ToList();
                    foreach (var questData in activeQuestList)
                    {
                        mstQuestData = questData;
                        hasQuest = true;
                        var actConsume = questData.actConsume;
                        if (actConsume > ap)
                        {
                            minAp = actConsume < minAp ? actConsume : minAp;
                            continue;
                        }
                        questId = questData.id;
                        break;
                    }

                    ///剧情check
                    if (!hasQuest && task.questIds.Contains("1000000"))
                    {
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
                            if (quest.actConsume < ap)
                            {
                                questId = quest.id;
                                mstQuestData = quest;
                                break;
                            }
                            else
                                minAp = quest.actConsume < minAp ? quest.actConsume : minAp;
                        }
                        if (!hasQuest)
                        {
                            await Context.ActorOf<RoleChapteredActor>().Ask<bool>(task.role.id);
                            logger.Info(task.role.username + " 章节清除完成");
                        }
                    }

                    if (!hasQuest)
                    {
                        if (serverApi.userQuest == null || serverApi.userQuest.Count < 3)
                        {
                            Log(task.id, GameAction.BATTLESETUP, "找不到匹配的副本。尝试重新获取");
                            await taskResultActor.Ask(new TaskResult()
                            {
                                id = task.id,
                                nextAction = GameAction.unknown,
                                state = TaskState.STOP,
                                nextStartTime = DateTime.Now
                            });
                            Sender.Tell(true);
                            return;
                        }
                        var mstQuests = AssetManage.Database.mstQuest.Where(q => task.questIds.Contains(q.id)).ToList();
                        if (mstQuests.Any(q => AssetManage.Database.mstQuestRelease.Where(p => p.questId == q.id).Any(p => p.type == 13 || p.type == 11)))
                        {
                            Log(task.id, GameAction.BATTLESETUP, "找不到匹配的副本。等待一小时后启动");
                            await taskResultActor.Ask(new TaskResult()
                            {
                                id = task.id,
                                nextAction = GameAction.unknown,
                                state = TaskState.STOP,
                                nextStartTime = DateTime.Now.AddHours(1)
                            });
                        }
                        else
                        {
                            Log(task.id, GameAction.BATTLESETUP, "找不到匹配的副本。任务结束");
                            await taskResultActor.Ask(new TaskResult()
                            {
                                id = task.id,
                                nextAction = GameAction.unknown,
                                state = TaskState.STOP,
                                enable = false
                            });
                        }
                        Sender.Tell(false);
                        return;
                    }

                    if (string.IsNullOrEmpty(questId))
                    {

                        var item = serverApi.userItem.Where(u => u.itemId == "100" || u.itemId == "102" || u.itemId == "101").FirstOrDefault();
                        if (item != null && item.num != 0 && task.useitem)
                        {
                            task.task_data["itemId"] = item.itemId;
                            task.task_data["num"] = "1";
                            if (await Context.ActorOf<ItemUseActor>().Ask<bool>(task))
                            {
                                Log(task.id, GameAction.BATTLESETUP, "Ap不足，消耗了一个苹果，总共" + item.num + "个");
                                task.action = GameAction.ITEMUSE;
                                Self.Tell(task);
                                return;
                            }
                        }
                        var t = TimeSpan.FromMilliseconds((minAp - ap) * 300 * 1000);
                        Log(task.id, GameAction.BATTLESETUP, "Ap不足，等待 " + t.Hours + "小时" + t.Minutes + "分" + t.Seconds + "秒");
                        if (t.TotalMinutes > 10)
                        {
                            await taskResultActor.Ask(new TaskResult()
                            {
                                id = task.id,
                                nextAction = GameAction.HOME,
                                state = TaskState.RUNNING
                            });
                        }
                        else
                        {
                            await Task.Delay(t);
                        }
                        Sender.Tell(true);
                        return;
                    }
                    var count = AssetManage.Database.mstQuestPhase.Where(p => p.questId == questId).Count();
                    var userQuest = serverApi.userQuest.FirstOrDefault(u => u.questId == questId);
                    if (count > 0 && userQuest != null && userQuest.questPhase < count)
                        task.task_data["questPhase"] = (++userQuest.questPhase).ToString();
                    else
                        task.task_data["questPhase"] = "1";


                    if (mstQuestData.type == "1")
                    {
                        var npcFlower = AssetManage.Database.npcFollower.FirstOrDefault(f => f.questId == questId && f.questPhase == task.task_data["questPhase"]);
                        if (npcFlower != null)
                        {
                            follower.id = npcFlower.id;
                            follower.index = 0;
                        }
                    }

                    task.task_data["questId"] = questId;
                    task.task_data["followerId"] = follower.id;
                    task.task_data["followerIndex"] = follower.index.ToString();
                    task.task_data["deckId"] = deckId;

                    Log(task.id, GameAction.BATTLESETUP, "战斗准备开始，战斗副本： " + mstQuestData.name);
                    var sleepTime = await Context.ActorOf<BattleSetupActor>().Ask<TimeSpan>(task);
                    if (sleepTime.TotalMilliseconds == 0)
                    {
                        Log(task.id, GameAction.BATTLESETUP, "战斗开始失败");
                        Sender.Tell(false);
                        return;
                    }
                    Log(task.id, GameAction.BATTLESETUP, "战斗开始成功，等待" + sleepTime.Minutes + "分" + sleepTime.Seconds + "秒");
                    await Task.Delay(sleepTime);
                    task.action = GameAction.BATTLESETUP;
                    Self.Tell(task, Sender);
                });
        }

        protected void BattleResult()
        {
            Receive<BattleTask>(
               new Predicate<BattleTask>(task => task.action == GameAction.BATTLERESULT), task =>
               {
                   taskResultActor.Tell(new TaskResult()
                   {
                       id = task.id,
                       nextAction = GameAction.unknown,
                       state = 0
                   });
                   Sender.Tell(true);
               });
        }

        protected void EndBattle()
        {
            ReceiveAsync<BattleTask>(
                 new Predicate<BattleTask>(task => task.action == GameAction.BATTLESETUP),
                 async task =>
                 {
                     var serverApi = task.serverApi;
                     if (serverApi == null || task.excuteType == ExcuteType.ErrorReStart || task.excuteType == ExcuteType.Expires)
                         Log(task.id, GameAction.BATTLESETUP, "正在尝试恢复战斗");
                     if (await Context.ActorOf<BattleResultActor>().Ask<bool>(task))
                     {
                         Log(task.id, GameAction.BATTLESETUP, "战斗计划结束");
                         task.action = GameAction.unknown;
                         task.excuteType = ExcuteType.Normal;
                         taskResultActor.Tell(new TaskResult()
                         {
                             id = task.id,
                             nextAction = GameAction.unknown,
                             state = 0,
                             nextStartTime = DateTime.Now.AddSeconds(5)
                         });
                         Sender.Tell(true);
                         return;
                     }
                     if (serverApi == null || task.excuteType == ExcuteType.ErrorReStart || task.excuteType == ExcuteType.Expires)
                         Log(task.id, GameAction.BATTLESETUP, "恢复战斗战斗失败");

                     await taskResultActor.Ask(new TaskResult()
                     {
                         id = task.id,
                         nextAction = GameAction.unknown,
                         state = TaskState.STOP,
                     });
                     Sender.Tell(false);
                 });
        }

        public BattleTaskActor()
        {
            Login();

            BattleSetup();

            BattleResult();

            EndBattle();
        }
    }

    public class Followerinfo
    {
        public Followerinfo()
        {
        }

        public string id { get; set; }
        public int index { get; set; }
        public string message { get; internal set; }
    }
}
