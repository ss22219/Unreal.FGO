using Akka.Actor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.TaskService.Actor;
using Unreal.FGO.Helper;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Core;
using Unreal.FGO.Common;
using Akka.Configuration;
using Unreal.FGO.TaskService.TaskExcutor;
using System.IO;
using log4net.Config;
using static AkkaMain;
using Topshelf;
using log4net;

namespace Unreal.FGO.TaskService
{
    public class SystemControl : ServiceControl
    {
        public ActorSystem system;
        ILog logger = LogManager.GetLogger(typeof(SystemControl));

        public bool Bootstrap()
        {
            Db Db = new Db();
            foreach (var item in Db.systemInfos.AsNoTracking().ToList())
            {
                if (item.name == "version")
                    AppInfo.version = item.value;
                else if (item.name == "dateVer")
                    AppInfo.dateVer = item.value;
                else if (item.name == "dataVer")
                    AppInfo.dataVer = item.value;
                else if (item.name == "appVer")
                    AppInfo.appVer = item.value;
            }

            AssetManage.LoadDatabase(AppInfo.dataVer);
            var admin = Db.users.Where(u => u.username == "super_admin").FirstOrDefault();
            Db.Dispose();
            var config = ConfigurationFactory.ParseString(@"
                    akka {  
                        actor {
                            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        }
                        remote {
                            helios.tcp {
                                transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                applied-adapters = []
                                transport-protocol = tcp
                                port = " + Const.TaskServiceSystemPort + @"
                                hostname = localhost
                            }
                        }
                    }
                    ");
            try
            {
                system = ActorSystem.Create(Const.TaskService, config);
                {
                    system.ActorOf<TaskCheckeActor>().Tell("");
                    system.ActorOf<Login>(Const.LoginActor);
                    system.ActorOf<BattleSetup>(Const.BattleSetupActor);
                    system.ActorOf<BattleResult>(Const.BattleResultActor);
                    if (admin != null)
                    {
                        //system.ActorOf<AdminRoleInit>().Tell(admin.id);
                        //system.ActorOf<AdminRoleLogin>().Tell(admin.id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("System Bootstrap Fail",ex);
                return false;
            }
            return true;
        }

        public bool Start(HostControl hostControl)
        {
            this.Bootstrap();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            system.Terminate().Wait();
            return true;
        }
    }
}
