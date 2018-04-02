using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Common.ActorResult
{
    public class BattleResultResult
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class UserQuest
    {
        public string questId { get; set; }
        public int phase { get; set; }
        public int clearNum { get; set; }
    }
}
