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
    public class BattleSetup : TaskExcutorBase
    {
        public BattleSetup()
        {
            ReceiveAsync<BattleSetupParam>(async param =>
            {
                var db = new Db();
                var result = new BattleSetupResult();
                try
                {
                    var role = db.GetUserRoleById(param.roleId);
                    var device = getDevice(role.device_id);

                    db.Dispose();
                    var questChecker = new QuestChecker();
                    var loginRes = await Context.ActorOf<InitRole>().Ask<LoginResult>(new InitRoleParam()
                    {
                        role = role
                    });

                    if (loginRes.code != 0)
                    {
                        result.code = -500;
                        result.message = loginRes.message;
                        Sender.Tell(result);
                        return;
                    }

                    var roleData = getRoleData(role.id);
                    if (roleData.quest_info == null)
                    {
                        result.code = -1;
                        result.message = "获取副本信息失败，该账号可能没有创建角色";
                        Sender.Tell(result);
                        return;
                    }
                    if (roleData.deck_info == null)
                    {
                        result.code = -1;
                        result.message = "获取队伍信息失败，该账号可能没有创建角色";
                        Sender.Tell(result);
                        return;
                    }
                    var userDeck = JsonConvert.DeserializeObject<List<ToploginUserdeck>>(roleData.deck_info);
                    var serverApi = InitServerApi(role, device, roleData);
                    questChecker.TimetampSecond = serverApi.TimetampSecond;
                    questChecker.userQuest = JsonConvert.DeserializeObject<List<HomeUserquest>>(roleData.quest_info);
                    var questList = questChecker.GetUserQuestList();
                    var mstQuest = questList.FirstOrDefault(q => q.id == param.questId);
                    logger.Info(serverApi.userGame.name + "  lv : " + serverApi.userGame.lv + "    " + "ap : " + serverApi.ApNum);

                    var ap = serverApi.ApNum;
                    if (mstQuest == null && param.questId == "1000000")
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
                            if (quest.actConsume < ap)
                            {
                                logger.Info("剧情：" + war.name + " " + spot.name + " " + quest.name);
                                mstQuest = quest;
                                param.questId = quest.id;
                                break;
                            }
                            else
                            {
                                result.code = -10;
                                result.message = "Ap不足";
                                Sender.Tell(result);
                                return;
                            }
                        }
                        if (mstQuest == null)
                        {
                            db = new Db();
                            role = db.userRoles.Find(role.id);
                            role.chaptered = true;
                            db.SaveChanges();
                            db.Dispose();
                            logger.Info(role.username + " 章节清除完成");
                        }
                    }
                    if (mstQuest == null)
                    {
                        result.code = -2;
                        result.message = "没有找到该副本，可能副本没有开启";
                        Sender.Tell(result);
                        return;
                    }
                    if (mstQuest.actConsume > ap)
                    {
                        result.code = -10;
                        result.message = "Ap不足";
                        Sender.Tell(result);
                        return;
                    }

                    var deckId = userDeck.First().id.ToString();
                    var phase = "1";
                    var phaseCount = AssetManage.Database.mstQuestPhase.Where(p => p.questId == param.questId).Count();
                    var userQuest = serverApi.userQuest.FirstOrDefault(u => u.questId == param.questId);
                    if (phaseCount > 0 && userQuest != null && userQuest.questPhase < phaseCount)
                        phase = (++userQuest.questPhase).ToString();

                    if (!await Context.ActorOf<CheckDeckActor>().Ask<bool>(new LoginTask()
                    {
                        role = role,
                        serverApi = serverApi,
                        roleData = roleData,
                        device = device
                    }))
                    {
                        result.code = -4;
                        result.message = "设置队伍失败";
                        Sender.Tell(result);
                        return;
                    }

                    var npcFlower = AssetManage.Database.npcFollower.FirstOrDefault(f => f.questId == mstQuest.id && f.questPhase == phase);
                    var flowerId = npcFlower == null ? "0" : npcFlower.id;
                    var setup = await serverApi.Battlesetup(deckId, flowerId, param.questId, phase);

                    if (setup.code != 0)
                    {
                        logger.Error("战斗开始失败");
                        logger.Error(setup.message);
                        logger.Error(setup.RequestMessage);
                        result.code = -500;
                        result.message = "战斗开始失败";
                        Sender.Tell(result);
                        return;
                    }
                    result.battleId = setup.cache.replaced.battle[0].id;
                    db = new Db();
                    roleData = db.roleData.Find(roleData.id);
                    roleData.battle_id = result.battleId;
                    role = db.userRoles.Find(role.id);
                    role.last_update_time = DateTime.Now;
                    roleData.usk = setup.response[0].usk;
                    db.SaveChanges();
                    db.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    result.code = -500;
                    result.message = ex.Message;
                }
                Sender.Tell(result);
            });
        }
    }
}
