using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
namespace Unreal.FGO.TaskService.Actor
{
    public class ItemUseActor : ActorBase
    {
        public ItemUseActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                if (task.serverApi == null)
                    task.serverApi = InitServerApi(task);
                var serverApi = task.serverApi;
                var itemId = task.task_data["itemId"];
                var num = task.task_data["num"];
                var result = await serverApi.Itemuse(itemId, num);
                if (result.code != 0)
                {
                    await TaskErrorBack(task.id, GameAction.ITEMUSE, result);
                    return;
                }

                Log(task.id, GameAction.LOGINCENTER, "使用苹果成功");
                await SaveActionData(task, GameAction.ITEMUSE);
                Sender.Tell(true);
            });
        }
    }
}
