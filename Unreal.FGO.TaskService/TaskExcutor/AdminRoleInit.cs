using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory;
using Unreal.FGO.TaskService.Actor;
using Akka.Actor;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class AdminRoleInit : TaskExcutorBase
    {
        public static List<int> Inited = new List<int>();
        public async Task RegistAndInit(user_role role)
        {
            var initRole = Context.ActorOf<InitRole>();
            if (!role.registed)
            {
                var registRes = await Context.ActorOf<Regist>().Ask<RegistResut>(new RegistParam()
                {
                    username = role.username
                });
                if (registRes.code != 0)
                {
                    logger.Error("注册失败：" + registRes.message);
                    return;
                }
            }

            var res = await initRole.Ask<LoginResult>(role);
            if (res.code != 0)
            {
                logger.Error("初始失败：" + res.message);
            }
        }
        public AdminRoleInit()
        {
            ReceiveAsync<int>(async adminId =>
            {
            start:
                var db = new Db();
                var date = DateTime.Now.Date;
                var roles = (from role in db.userRoles
                             where role.registed == false
                             select role).ToList();
                db.Dispose();
                var taskList = new List<Task>();
                foreach (var role in roles)
                {
                    if (Inited.IndexOf(role.id) != -1)
                        continue;
                    Inited.Add(role.id);
                    taskList.Add(RegistAndInit(role));
                }
                if (taskList.Count > 0)
                    await Task.Factory.ContinueWhenAll(taskList.ToArray(), (results) => { });
                await Task.Delay(TimeSpan.FromSeconds(10));
                goto start;
            });
        }
    }
}
