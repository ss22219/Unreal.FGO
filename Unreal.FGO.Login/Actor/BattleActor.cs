using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Core.Api;
using Unreal.FGO.Core;

namespace Unreal.FGO.Console.Actor
{
    public class BattleTask : LoginTask
    {
        public bool useitem { get; set; }
        public string deckId { get; set; }
        public string followerId { get; set; }
        public string[] questIds { get; set; }
    }
    public class BattleActor : ActorBase
    {
        public BattleActor()
        {
            ReceiveAsync<BattleTask>(async task =>
            {
                if (!await Context.ActorOf<LoginActor>().Ask<bool>(task))
                {
                    Sender.Tell(false);
                    return;
                }
                if (!await Context.ActorOf<HomeActor>().Ask<bool>(task))
                {
                    Sender.Tell(false);
                    return;
                }

                var serverApi = task.serverApi;
                var deckId = string.IsNullOrEmpty(task.deckId) ? serverApi.userDeck.First().id : task.deckId.ToString();
                var followerId = string.IsNullOrEmpty(task.followerId) ? serverApi.userFollower.First().userId : task.followerId.ToString();

                System.Console.WriteLine("userId : " + serverApi.PlatfromInfos["userId"]);
                System.Console.WriteLine("name : " + serverApi.userGame.name);
                System.Console.WriteLine("lv : " + serverApi.userGame.lv);
                System.Console.WriteLine("ap : " + serverApi.ApNum);
                start:
                bool hasQuest = false;
                string questId = null;
                int ap = serverApi.ApNum;
                int minAp = 10000000;
                mstQuest mstQuestData = null;
                foreach (var item in serverApi.userQuest)
                {
                    if (task.questIds.Contains(item.questId))
                    {
                        var questData = AssetManage.Database.mstQuest.FirstOrDefault(q => q.id == item.questId);
                        if (questData != null)
                        {
                            mstQuestData = questData;
                            hasQuest = true;
                            var actConsume = int.Parse(questData.actConsume); ;
                            if (actConsume > ap)
                            {
                                minAp = actConsume < minAp ? actConsume : minAp;
                                continue;
                            }
                            questId = item.questId;
                            break;
                        }
                    }
                }
                if (!hasQuest)
                {
                    System.Console.WriteLine("战斗任务找不到匹配的副本。自动禁用 ID:" + task.id);
                    await Context.ActorOf<DisableTaskAcotr>().Ask(task.id);
                    Sender.Tell(false);
                    return;
                }
                if (string.IsNullOrEmpty(questId))
                {
                    System.Console.WriteLine("Ap不够了");
                    var item = serverApi.userItem.Where(u => u.itemId == "100").FirstOrDefault();
                    if (item != null && item.num != "0" && task.useitem)
                    {
                        var itemuse = await serverApi.Itemuse(item.itemId, "1");
                        if (itemuse.code == 0)
                        {
                            System.Console.WriteLine("消耗了一个苹果，总共" + item.num + "个");
                            goto start;
                        }
                    }
                    var t = TimeSpan.FromMilliseconds((minAp - ap) * 300 * 1000);
                    System.Console.WriteLine("等待 " + t.Hours + "小时" + t.Minutes + "分" + t.Seconds + "秒");
                    await Task.Delay(t);
                }
                var setup = await serverApi.Battlesetup(deckId, followerId, questId);
                if (setup.code != 0)
                {
                    await TaskErrorAndBack(task.id, GameAction.BATTLESETUP, setup);
                    return;
                }

                var random = new Random();
                var time = random.Next((int)TimeSpan.FromMinutes(2).TotalMilliseconds, (int)TimeSpan.FromMinutes(3).TotalMilliseconds);
                var timespan = TimeSpan.FromMilliseconds(time);

                Context.ActorOf<SaveRoleDataActor>().Tell(new SaveActionDataTask()
                {
                    action = GameAction.BATTLESETUP,
                    serverApi = serverApi,
                    expiresTime = DateTime.Now.Add(timespan),
                    taskId = task.id,
                    battleId = setup.cache.replaced.battle[0].id
                });

                var color = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.DarkBlue;
                System.Console.WriteLine(mstQuestData.name);
                System.Console.WriteLine("战斗开始，等待" + timespan.Minutes + "分" + timespan.Seconds + "秒");
                System.Console.ForegroundColor = color;
                await Task.Delay(time);
                var count = random.Next(20, 35);
                var action = new BattleresultActionItem[count];
                for (int i = 0; i < count; i++)
                {
                    action[i] = new BattleresultActionItem()
                    {
                        ty = random.Next(1, 4),
                        uid = random.Next(1, 4)
                    };
                }
                var battresult = await serverApi.Battleresult(setup.cache.replaced.battle[0].id, action, new string[] { });
                if (battresult.code != 0)
                {
                    await TaskErrorAndBack(task.id, GameAction.BATTLERESULT, battresult);
                    return;
                }
                System.Console.WriteLine("战斗结束");
                await Task.Delay(10000);
                Context.ActorOf<SaveRoleDataActor>().Tell(task);
                Sender.Tell(true);
            });
        }
    }
}
