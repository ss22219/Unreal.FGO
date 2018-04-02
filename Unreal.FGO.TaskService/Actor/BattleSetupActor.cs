using System;
using Akka.Actor;

namespace Unreal.FGO.TaskService.Actor
{
    public class BattleSetupActor : ActorBase
    {
        public BattleSetupActor()
        {
            ReceiveAsync<BattleTask>(async task =>
            {
                var deckId = task.task_data["deckId"];
                var followerId = task.task_data["followerId"];
                var followerIndex = "0";
                if (task.task_data.ContainsKey("followerIndex"))
                    followerIndex = task.task_data["followerIndex"];
                var questId = task.task_data["questId"];
                var questPhase = task.task_data["questPhase"];

                if (task.serverApi == null)
                    task.serverApi = InitServerApi(task);
                var serverApi = task.serverApi;

                var setup = await serverApi.Battlesetup(deckId, followerId, questId, questPhase, followerIndex);
                if (setup.code != 0)
                {
                    await TaskError(task.id, GameAction.BATTLESETUP, setup);
                    Sender.Tell(new TimeSpan(0));
                    return;
                }

                var random = new Random();
                var time = random.Next((int)TimeSpan.FromMinutes(1).TotalMilliseconds, (int)TimeSpan.FromMinutes(2).TotalMilliseconds);
                var timespan = TimeSpan.FromMilliseconds(time);

                task.task_data["battleId"] = setup.cache.replaced.battle[0].id;
                task.roleData.battle_id = setup.cache.replaced.battle[0].id;
                await SaveActionData(task, GameAction.BATTLESETUP, DateTime.Now.Add(timespan));
                Sender.Tell(timespan);
            });
        }
    }
}
