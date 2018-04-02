using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Common
{
    public class Const
    {
        public const string WebApi = "WebApi";
        public const string TaskService = "TaskService";
        public const string LoginResultActor = "LoginResult";
        public const int TaskServiceSystemPort = 24001;
        public const int WebApiSystemPort = 24002;

        public static string LoginActor = "Login";

        public static string BattleSetupActor = "BattleSetup";
        public static string BattleResultActor = "BattleResult";
    }
}
