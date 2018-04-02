using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Core.Api;
using Newtonsoft.Json;

namespace Unreal.FGO.TaskService.Actor
{
    public class BatleData
    {
        public string actions { get; set; }
        public string voicePlayedList { get; set; }
    }

    public class BattleResultActor : ActorBase
    {
        private static Dictionary<string, BatleData> constBattleData = new Dictionary<string, BatleData>() {
            { "1000000", new BatleData() { actions = "{ \"logs\":[{\"uid\":1,\"ty\":3},{\"uid\":2,\"ty\":2},{\"uid\":3,\"ty\":1},{\"uid\":1,\"ty\":3},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":2},{\"uid\":3,\"ty\":3},{\"uid\":2,\"ty\":3},{\"uid\":2,\"ty\":3},{\"uid\":1,\"ty\":1},{\"uid\":2,\"ty\":1},{\"uid\":3,\"ty\":1},{\"uid\":3,\"ty\":3},{\"uid\":2,\"ty\":2},{\"uid\":1,\"ty\":2}]}", voicePlayedList = "[[100100,1],[100100,4]]"} },
            { "1000001", new BatleData() { actions = "{ \"logs\":[{\"uid\":1,\"ty\":3},{\"uid\":1,\"ty\":2},{\"uid\":1,\"ty\":5},{\"uid\":1,\"ty\":2},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":3},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":2}]}", voicePlayedList = "[]"} },
            { "1000002", new BatleData() { actions = "{ \"logs\":[{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":3},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":2},{\"uid\":1,\"ty\":2},{\"uid\":1,\"ty\":5},{\"uid\":1,\"ty\":3},{\"uid\":1,\"ty\":1},{\"uid\":1,\"ty\":2}]}", voicePlayedList = "[]"} },
        };

        public BattleResultActor()
        {
            ReceiveAsync<BattleTask>(async task =>
            {
                try
                {
                    if (task.serverApi == null)
                        task.serverApi = InitServerApi(task);
                    var serverApi = task.serverApi;

                    if (!task.task_data.ContainsKey("battleId"))
                    {
                        Log(task.id, GameAction.BATTLERESULT, "结束战斗失败，战斗信息不存在");
                        await TaskErrorBack(task.id, GameAction.BATTLERESULT, serverApi.LastResponse, "无法读取战斗ID");
                        return;
                    }
                    BattleresultRes result = null;
                    var battleId = task.task_data["battleId"];

                    battleEnd:
                    if (task.task_data.ContainsKey("questId") && constBattleData.ContainsKey(task.task_data["questId"]))
                    {
                        var data = constBattleData[task.task_data["questId"]];
                        result = await serverApi.Battleresult(battleId, data.actions, data.voicePlayedList);
                    }
                    else
                    {
                        var random = new Random();
                        var count = random.Next(10, 35);
                        var action = new BattleresultActionItem[count];

                        for (int i = 0; i < count; i++)
                        {
                            action[i] = new BattleresultActionItem()
                            {
                                ty = random.Next(1, 4),
                                uid = random.Next(1, 4)
                            };
                        }
                        result = await serverApi.Battleresult(battleId, action, "[]");
                    }
                    if (result.code != 0)
                    {
                        await TaskErrorBack(task.id, GameAction.BATTLERESULT, result);
                        return;
                    }
                    if (result.response[0].nid == "battle_setup")
                    {
                        await Task.Delay(1000);
                        goto battleEnd;
                    }
                    if (task.roleData != null && serverApi.userQuest != null)
                        task.roleData.quest_info = JsonConvert.SerializeObject(serverApi.userQuest);
                    Log(task.id, GameAction.BATTLESETUP, "结束战斗成功");
                    await SaveActionData(task, GameAction.BATTLERESULT);
                    Sender.Tell(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Log(task.id, GameAction.BATTLERESULT, "结束战斗失败");
                    Sender.Tell(false);
                }
            });
        }
    }
}
