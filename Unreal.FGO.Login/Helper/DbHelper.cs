using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.Helper
{
    public static class DbHelper
    {
        public static Db DB
        {
            get
            {
                var db = CallContext.GetData("DbContext") as Db;
                if (db == null)
                {
                    db = new Db();
                    CallContext.SetData("DbContext", db);
                }
                return db;
            }
        }
    }
}