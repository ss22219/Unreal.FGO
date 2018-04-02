using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unreal.FGO.Repostory;
using System.Threading.Tasks;
using Unreal.FGO.Common.ActorParam;

namespace Unreal.FGO.WebApi
{
    [HubName("fgo")]
    public class Connection : Hub
    {
        public static Connection connection;
        public Dictionary<string, string> userConnects = new Dictionary<string, string>();
        public Connection()
        {
            connection = this;
        }

        public void RoleLoginResult(LoginResultParam result)
        {

            using (var db = new Db())
            {
                var role = db.GetUserRoleById(result.roleId);
                if (role != null)
                {
                    var user = db.GetUserById(role.user_id);
                    if (user != null && userConnects.ContainsKey(user.token))
                    {
                        var client = Clients.Client(userConnects[user.token]);
                        if (client != null)
                            client.RoleLoginResult(result);
                    }
                }
            }
        }

        public void Login(string token)
        {
            using (var db = new Db())
            {
                var user = db.users.Where(u => u.token == token);
                if (user != null)
                {
                    userConnects[token] = Context.ConnectionId;
                }
            }
        }
    }
}