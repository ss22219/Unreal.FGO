using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Core;

namespace Unreal.FGO.Helper
{
    public class ServerApiManager
    {
        private static ConcurrentDictionary<int, ServerApi> serverApiMap = new ConcurrentDictionary<int, ServerApi>();
        public static ConcurrentDictionary<int, ServerApi> GetInstance()
        {
            return serverApiMap;
        }
        public static ServerApi GetByRoleId(int roleId)
        {
            if (!GetInstance().ContainsKey(roleId))
            {
                return null;
            }
            return serverApiMap[roleId];
        }

        public static void SetServerApi(int roleId, ServerApi serverApi)
        {
            GetInstance()[roleId] = serverApi;
        }
    }
}
