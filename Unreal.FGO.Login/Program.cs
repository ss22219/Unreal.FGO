using Akka.Actor;
using Disunity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unreal.FGO.Helper;
using Unreal.FGO.Core;
using Unreal.FGO.Core.Api;
using AutoMapper;
using Unreal.FGO.Common;
using Unreal.FGO.TaskService.TaskExcutor;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Repostory;
using Akka.Configuration;
using Unreal.FGO.Repostory.Model;
using log4net.Config;
using log4net;
using Akka.Event;
using Akka.Remote;
using System.Diagnostics;
using System.Reflection;

namespace Unreal.FGO
{
    class Program
    {
        static ActorSystem system;

        private static void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }
        public static int taskCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["taskCount"]);
        public static int dataCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["dataCount"]);
        public static int taskDelay = int.Parse(System.Configuration.ConfigurationManager.AppSettings["taskDelay"]);
        public static bool exit = false;
        public static int loginError = 0;
        public static int threadEnd = 0;
        public static bool registError = false;
        static ILog logger = LogManager.GetLogger(typeof(Program));

        public static user admin { get; private set; }

        public class DeadActor : ActorBase
        {
            protected override bool Receive(object message)
            {
                if (!Process.GetProcesses().Any(p => p.ProcessName.ToLower().IndexOf("namecollect") != -1))
                {
                    var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    Process.Start(dir + "/Unreal.Bili.NameCollect.exe");
                }
                return true;
            }
        }

        static void Main(string[] args)
        {
            if (!Process.GetProcesses().Any(p => p.ProcessName.ToLower().IndexOf("namecollect") != -1))
            {
                var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Process.Start(dir + "/Unreal.Bili.NameCollect.exe");
            }
            InitLog4Net();
            var config = ConfigurationFactory.ParseString(@"
                            akka {  
                                stdout-loglevel = DEBUG
                                loglevel = DEBUG
                                actor {
                                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                                }
                                remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        applied-adapters = []
                                        transport-protocol = tcp
                                        hostname = localhost
                                        port = 0
                                    }
                                    client {
                                          reconnect-delay = 1            # in seconds (5 sec default)
                                          read-timeout = 10              # in seconds (10 sec default)
                                          reconnection-time-window = 0 # the maximum time window that a client should try to reconnect for
                                    }
                                }
                            }").WithFallback(ConfigurationFactory.ParseString("akka.remote.retry-gate-closed-for = 60s"));
            system = ActorSystem.Create("console", config);
            system.EventStream.Subscribe(system.ActorOf<DeadActor>(), typeof(DeadLetter));
            Db db = null;
            try
            {
                db = new Db();
                admin = db.users.Where(u => u.username == "super_admin").FirstOrDefault();
            }
            catch (Exception ex)
            {
                CloseApp();
                return;
            }

            var dataVer = db.GetSystemInfoById("dataVer").value;
            var time = DateTime.Now.Date;
            var roles = db.userRoles.AsNoTracking().Where(u => (u.user_id == 0 && u.inited == false) || (u.user_id == admin.id && u.last_update_time < time)).OrderBy(u => u.last_task_time).Take(dataCount).ToList();
            if (roles.Count > 0)
            {
                var ids = roles.Select(r => r.id.ToString()).Aggregate((a, b) => a + "," + b);
                db.Database.ExecuteSqlCommand("update user_role set last_task_time=@p0 where id in(" + ids + ")", DateTime.Now);
                db.Dispose();
            }
            Console.WriteLine("加载资料库");
            AssetManage.LoadDatabase(dataVer);

            Console.WriteLine("需要" + roles.Count + "次初始");
            int i = 0;
            if (roles.Count > 0)
            {
                for (int k = 0; k < taskCount; k++)
                {
                    Task.Factory.StartNew(async () =>
                    {
                        bool end = false;
                        user_role role = null;
                    start:
                        Monitor.Enter(typeof(Program));
                        if (!registError && i < roles.Count)
                        {
                            role = roles[i];
                            i++;
                        }
                        else
                            end = true;
                        Monitor.Exit(typeof(Program));

                        if (!end)
                        {
                            await Init(role);
                            goto start;
                        }

                        Interlocked.Increment(ref threadEnd);
                        logger.Info("Task" + threadEnd + " End");
                        if (threadEnd >= taskCount)
                            CloseApp();
                    });
                    if (taskDelay > 0)
                        Thread.Sleep(taskDelay);
                }
            }
            else
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                CloseApp();
                return;
            }
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromHours(1));
                CloseApp();
            });
            while (!exit)
                Thread.Sleep(TimeSpan.FromSeconds(5));
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        private static void CloseApp()
        {
            if (exit)
                return;
            exit = true;
            try
            {
                var db = new Db();
                db.Database.ExecuteSqlCommand(@"update devices set user_id=@p0 where user_id=0", admin.id);
                db.Database.ExecuteSqlCommand(@"update user_role set user_id=@p0 where user_id=0 and inited=1", admin.id);
                db.Dispose();
            }
            catch (Exception)
            {
            }
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public class GetName : ReceiveActor
        {
            public GetName()
            {
                ReceiveAnyAsync(async obj =>
                {
                    logger.Info("请求名字中");
                    try
                    {
                        var actor = Context.ActorSelection("akka.tcp://name@localhost:20023/user/GetName");
                        var result = await actor.Ask<string>(obj, TimeSpan.FromSeconds(3));
                        logger.Info("名字接受成功");
                        Sender.Tell(result);
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        logger.Error("名字接受失败");
                        Sender.Tell(null);
                    }
                });
            }
        }

        public static int nameError;
        private static async Task Init(user_role role)
        {
            logger.Info(role.username);
        getName:
            string name = null;
            if (!role.inited)
            {
                logger.Info("名字获取");
                name = await system.ActorOf<GetName>().Ask<string>(1);
                if (name == null)
                {
                    Interlocked.Increment(ref nameError);
                    if (nameError > 50)
                    {
                        var proccess = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToLower().IndexOf("namecollect") != -1);
                        if (proccess != null)
                            proccess.Kill();
                        Interlocked.Exchange(ref nameError, 0);
                    }
                    logger.Error("名字获取失败");
                    await Task.Delay(5000);
                    goto getName;
                }
                nameError = 0;
                logger.Info("名字:" + name);
            }
            else
            {
                logger.Info("上次登陆时间：" + role.last_update_time.ToString("yyyy-MM-dd hh:mm"));
            }
            var result = await system.ActorOf<InitRole>().Ask<LoginResult>(new InitRoleParam()
            {
                name = name,
                role = role
            });
            if (result.code != 0) //message = "用户名已经存在"
            {
                if (result.message != "用户名已经存在" || result.message != "用户名中包含敏感词")
                    Interlocked.Increment(ref loginError);
                else
                    goto getName;
                logger.Info(name + "处理失败");
                logger.Error(result.message);
                if (result.message == "获取游戏版本失败")
                {
                    registError = true;
                    return;
                }
                if (loginError > 10)
                {
                    registError = true;
                    return;
                }
                goto getName;
            }
            else
            {
                Interlocked.Exchange(ref loginError, 0);
                logger.Info(name + "处理成功");
                return;
            }
        }

    }
}
