using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;
using Unreal.FGO.Common.ActorParam;

namespace Unreal.FGO.WebApi.Actor
{
    public class LoginResultActor : ReceiveActor
    {
        public LoginResultActor()
        {
            Receive<LoginResultParam>(result =>
            {
                if (Connection.connection != null)
                    Connection.connection.RoleLoginResult(result);
            });
        }
    }
}