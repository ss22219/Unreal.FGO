using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Repostory;
using Unreal.FGO.Helper;
using Unreal.FGO.Core.Api;
using Newtonsoft.Json;
using log4net;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.TaskService.Actor;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class BattleResult : TaskExcutorBase
    {
        public BattleResult()
        {
            ReceiveAsync<BattleResultParam>(async param =>
            {
                var db = new Db();
                var role = db.GetUserRoleById(param.roleId);
                db.Dispose();

                var result = new BattleResultResult();
                var device = getDevice(role.device_id);
                var roleData = getRoleData(role.id);
                var serverApi = InitServerApi(role, device, roleData);

                var taskData = new Dictionary<string, string> {
                    { "battleId",param.battleId },
                };

                if (!string.IsNullOrEmpty(param.questId))
                    taskData["questId"] = param.questId;
                var task = new BattleTask()
                {
                    role = role,
                    device = device,
                    roleData = roleData,
                    serverApi = serverApi,
                    task_data = taskData
                };
                var flag = await Context.ActorOf<BattleResultActor>().Ask<bool>(task);

                if (!flag)
                {
                    result.code = -1;
                    result.message = "战斗结束失败";
                    Sender.Tell(result);
                    return;
                }
                Sender.Tell(result);
            });
        }
    }
}
