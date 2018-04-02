using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Unreal.FGO.Helper;
using Unreal.FGO.Core;

namespace Unreal.FGO.TaskService.Actor
{
    public class MemberActor : ActorBase
    {
        public MemberActor()
        {
            ReceiveAsync<LoginTask>(async task =>
            {
                try
                {
                    if (task.serverApi == null)
                        task.serverApi = InitServerApi(task);
                    var serverApi = task.serverApi;
                    var member = await serverApi.Member();
                    if (member.code == 0)
                    {
                        var version = member.response[0].success.version;
                        if (!string.IsNullOrEmpty(version) && version != serverApi.PlatfromInfos["version"])
                        {
                            serverApi.PlatfromInfos["version"] = version;
                            serverApi.PlatfromInfos["dataVer"] = version;
                            await updateVersionActor.Ask(new VersionInfo()
                            {
                                dateVer = version,
                                version = version
                            });
                        }
                    }
                    else
                    {
                        await TaskErrorBack(task.id, GameAction.MEMBER, member);
                        return;
                    }
                    Sender.Tell(true);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Sender.Tell(false);
                }
            });
        }
    }
}
