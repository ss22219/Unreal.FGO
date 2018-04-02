using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory.Model;
using Akka.Actor;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.Repostory;
using Newtonsoft.Json;
using Unreal.FGO.Core.Api;
using Unreal.FGO.Core;
using Unreal.FGO.TaskService.Actor;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class BoxGachaDraw : TaskExcutorBase
    {
        public BoxGachaDraw()
        {
            ReceiveAsync<user_role>(async role =>
            {
                var device = getDevice(role.device_id);
                var result = new LoginResult();
                var loginResult = await Context.ActorOf<Login>().Ask<LoginResult>(new LoginParam()
                {
                    device = device,
                    role = role
                });
                if (loginResult.code != 0)
                {
                    Sender.Tell(loginResult);
                    return;
                }
                var roleData = getRoleData(role.id);
                var serverApi = InitServerApi(role, device, roleData);
                var task = new LoginTask()
                {
                    device = device,
                    role = role,
                    serverApi = serverApi,
                    roleData = roleData
                };

                var home = await Context.ActorOf<HomeActor>().Ask<bool>(task);
                if (!home)
                {
                    result.code = -1;
                    result.message = "进入首页失败";
                    Sender.Tell(result);
                    return;
                }
                if (serverApi.userItem == null && roleData.user_item != null)
                    serverApi.userItem = JsonConvert.DeserializeObject<List<HomeUseritem>>(roleData.user_item);

                logger.Info("进入首页成功");

                if (serverApi.userItem == null)
                {
                    result.code = -1;
                    result.message = "用户道具信息获取失败";
                    Sender.Tell(result);
                    return;
                }
                var boxId = "800051";
                int boxBaseId = 0;
                var drawNum = 0;
                var itemId = "94001204";

                var boxGacha = AssetManage.Database.mstBoxGacha.Where(b => b.id == boxId).FirstOrDefault();
                if (serverApi.userBoxGacha != null && serverApi.userBoxGacha.Any(u => u.boxGachaId == boxId))
                {
                    var userBoxGacha = serverApi.userBoxGacha.Where(u => u.boxGachaId == boxId).FirstOrDefault();
                    drawNum = userBoxGacha.drawNum;
                    boxBaseId = userBoxGacha.boxIndex;
                }

                while (true)
                {
                    var boxNum = AssetManage.Database.mstBoxGachaBase.Where(b => b.id == boxGacha.baseIds[boxBaseId]).Sum(b => b.maxNum);
                    if (boxNum == 0)
                    {
                        logger.Info("boxNum为空");
                        result.code = -1;
                        Sender.Tell(result);
                        return;
                    }
                    var num = boxNum - drawNum > 10 ? 10 : boxNum - drawNum;

                    var item = serverApi.userItem.Where(i => i.itemId == itemId).FirstOrDefault();
                    if (item == null || item.num < 20)
                    {
                        logger.Warn("素材已经用完");
                        result.code = 0;
                        Sender.Tell(result);
                        return;
                    }
                    logger.Info("目前素材:" + item.num);
                    var drawResult = await serverApi.Boxgachadraw(boxId, num);
                    if (drawResult.code != 0)
                    {
                        logger.Error("抽奖出错");
                        logger.Error(drawResult.message);
                        logger.Error(drawResult.RequestMessage);
                        result.code = -500;
                        result.message = "抽奖出错";
                        Sender.Tell(result);
                        return;
                    }
                    boxBaseId = drawResult.cache.updated.userBoxGacha[0].boxIndex;
                    drawNum = drawResult.cache.updated.userBoxGacha[0].drawNum;
                    try
                    {
                        logger.Info("抽奖总数：" + drawResult.cache.updated.userBoxGacha[0].drawNum);
                    }
                    catch (Exception)
                    {
                    }

                    logger.Info("抽奖成功");
                    await Task.Delay(1000);

                    if (drawResult.cache.updated.userBoxGacha[0].isReset)
                    {
                        var nos = JsonConvert.DeserializeObject<int[]>(drawResult.cache.updated.userBoxGachaDeck[0].boxGachaBaseNo);
                        if (nos.Length == drawResult.cache.updated.userBoxGacha[0].drawNum)
                        {
                            logger.Info("抽奖完成一次，当前第" + drawResult.cache.updated.userBoxGacha[0].resetNum + "次");
                        }
                        else
                            continue;
                        var resetRes = await serverApi.Boxgachareset(boxId);
                        if (resetRes.code != 0)
                        {
                            logger.Error("抽奖重置出错");
                            logger.Error(resetRes.message);
                            logger.Error(resetRes.RequestMessage);
                            result.code = -500;
                            result.message = "抽奖重置出错";
                            Sender.Tell(result);
                            return;
                        }
                        boxBaseId = resetRes.cache.updated.userBoxGacha[0].boxIndex;
                        drawNum = 0;
                        logger.Info("抽奖重置成功");
                    }
                }
            });
        }
    }
}
