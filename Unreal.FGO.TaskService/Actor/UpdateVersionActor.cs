using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core;
using Unreal.FGO.Helper;
using Akka.Actor;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.TaskService.Actor
{
    public class VersionInfo
    {
        public string version { get; set; }
        public string dateVer { get; set; }
    }
    public class UpdateVersionActor : ActorBase
    {
        public UpdateVersionActor()
        {
            Receive<VersionInfo>(info =>
            {
                var db = new Db();
                if (AppInfo.version != info.version)
                {
                    logger.Info("更新数据库...");
                    AssetManage.LoadDatabase(info.version);
                    logger.Info("更新完成，版本:" + info.version);
                }
                AppInfo.version = info.version;
                AppInfo.dateVer = info.dateVer;
                AppInfo.dataVer = info.version;

                foreach (var item in db.systemInfos)
                {
                    if (item.name == "version")
                        item.value = info.version;
                    else if (item.name == "dateVer")
                        item.value = info.dateVer;
                    else if (item.name == "dataVer")
                        item.value = info.version;
                }
                db.SaveChanges();
                Sender.Tell(true);
                db.Dispose();
            });
        }
    }
}
