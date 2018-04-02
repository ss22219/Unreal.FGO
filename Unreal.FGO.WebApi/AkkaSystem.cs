using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Unreal.FGO.Common;
using Unreal.FGO.WebApi.Actor;

namespace Unreal.FGO.WebApi
{
    public class AkkaSystem
    {
        public static ActorSystem System;
        public static readonly string TaskServiceUrl = "akka.tcp://" + Const.TaskService + "@localhost:" + Const.TaskServiceSystemPort + "/user/";
        public static ActorSelection LoginActor()
        {
            return System.ActorSelection(TaskServiceUrl + Const.LoginActor);
        }

        public static ActorSelection BattleSetupActor()
        {
            return System.ActorSelection(TaskServiceUrl + Const.BattleSetupActor);
        }

        public static ActorSelection BattleResultActor()
        {
            return System.ActorSelection(TaskServiceUrl + Const.BattleResultActor);
        }
        public static void Init()
        {
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
                                        port = " + Const.WebApiSystemPort + @"
                                        hostname = localhost
                                    }
                                }
                            }");
            System = ActorSystem.Create(Const.WebApi, config);
            System.ActorOf<LoginResultActor>(Const.LoginResultActor);
        }
    }
}