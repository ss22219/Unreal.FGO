using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Core.Api
{
    public abstract class BaseResponse
    {
        public virtual string message { get; set; }
        public virtual int code { get; set; }
        public virtual string action { get; set; }
        public virtual string RequestMessage { get; set; }
    }

    public class fail
    {
        public string title { get; set; }
        public string detail { get; set; }
        public string action { get; set; }
    }


    public class success
    {
        public string rguid { get; set; }
        public string rgusk { get; set; }
        public int rgtype { get; set; }
        public string version { get; set; }
        public int dateVer { get; set; }
        public int dataVer { get; set; }
    }

}
