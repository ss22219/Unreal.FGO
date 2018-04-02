using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.Common.ActorParam
{
    public class LoginParam
    {
        public user_role role { get; set; }
        public device device { get; set; }
        public bool home { get; set; }
    }
}
