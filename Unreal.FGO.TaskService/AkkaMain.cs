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
using Topshelf;
using Unreal.FGO.TaskService;

public class AkkaMain
{
    private static void InitLog4Net()
    {
        var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
        XmlConfigurator.ConfigureAndWatch(logCfg);
    }

    public static void Main(string[] args)
    {
        InitLog4Net();
        HostFactory.Run(x =>
        {
                x.Service<SystemControl>();
                x.RunAsLocalSystem();
                x.SetDescription("Unreal.FGO.TaskService");
                x.SetDisplayName("Unreal.FGO.TaskService");
                x.SetServiceName("Unreal.FGO.TaskService");
        });
    }
}
