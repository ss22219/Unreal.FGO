using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core;
using Akka.Actor;
using Unreal.FGO.Core.Api;

namespace Unreal.FGO.TaskService.Actor
{
    public class SvtModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string lv { get; set; }
        public string limit { get; internal set; }
        public string svtId { get; internal set; }
        public int rarity { get; internal set; }
    }
    public class CheckDeckActor : ActorBase
    {
        public CheckDeckActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                var serverApi = task.serverApi;
                if (serverApi.userDeck == null)
                {
                    await TaskErrorBack(task.id, GameAction.DECKSETUP, null, "没有 userDeck");
                    return;
                }
                if (serverApi.userSvt == null)
                {
                    await TaskErrorBack(task.id, GameAction.DECKSETUP, null, "没有 userSvt");
                    return;
                }
                var deck = serverApi.userDeck[0];
                var userSvtIds = deck.deckInfo.svts.Select(s => s.userSvtId).ToList();
                var userId = serverApi.PlatfromInfos["userId"];
                if (deck.deckInfo.svts.Where(s => (s.isFollowerSvt == false || s.isFollowerSvt == null) && s.userSvtId != 0).Count() < 2)
                {
                    logger.Info("正在设置队伍");
                    var ndeck = new ToploginUserdeck()
                    {
                        id = deck.id,
                        name = deck.name,
                        deckNo = 0,
                        userId = 0,
                        deckInfo = new ToploginDeckinfo() {
                            svts = new List<ToploginSvts>()
                        }
                    };
                    for (int i = 1; i < 7; i++)
                    {
                        ndeck.deckInfo.svts.Add(new ToploginSvts()
                        {
                            id = i,
                            isFollowerSvt = false,
                            userId = 0,
                            userSvtEquipIds = new List<int>() { 0 },
                            userSvtId = 0
                        });
                    }
                    var svts = serverApi.userSvt.Where(s => !string.IsNullOrEmpty(s.id) && !userSvtIds.Contains(long.Parse(s.id))).ToList();
                    var models = new List<SvtModel>();
                    foreach (var svt in svts)
                    {
                        var model = new SvtModel()
                        {
                            id = svt.id,
                            svtId = svt.svtId,
                            lv = svt.lv,
                            limit = svt.limitCount,
                        };
                        var mstSvt = AssetManage.Database.mstSvt.FirstOrDefault(s => svt.svtId == s.id && s.type == "1");
                        if (mstSvt == null)
                            continue;
                        model.name = mstSvt.battleName;

                        var limit = AssetManage.Database.mstSvtLimit.FirstOrDefault(l => l.svtId == svt.svtId && l.limitCount == svt.limitCount);
                        if (limit != null)
                            model.rarity = limit.rarity;
                        models.Add(model);
                    }

                    var msvt = models.OrderByDescending(m => m.rarity).FirstOrDefault();
                    if (msvt != null)
                    {
                        logger.Info("从者设置为：" + msvt.name);
                        ndeck.deckInfo.svts[0].userSvtId = deck.deckInfo.svts[0].userSvtId;
                        ndeck.deckInfo.svts[1].userSvtId = long.Parse(msvt.id);
                        ndeck.deckInfo.svts[2].isFollowerSvt = true;
                        var result = await serverApi.Decksetup(new List<Core.Api.ToploginUserdeck>() { ndeck });
                        if (result.code != 0)
                        {
                            await TaskErrorBack(task.id, GameAction.DECKSETUP, result);
                        }
                        else
                        {
                            logger.Info("正在队伍设置成功");
                            Sender.Tell(true);
                            return;
                        }
                    }
                    await TaskErrorBack(task.id, GameAction.DECKSETUP, null, "获取从者失败");
                    Sender.Tell(false);
                    return;
                }
                Sender.Tell(true);
            });
        }
    }
}
