using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory;
using Unreal.FGO.TaskService.Actor;
using Akka.Actor;
namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class AdminRoleLogin : TaskExcutorBase
    {
        public AdminRoleLogin()
        {
            ReceiveAsync<int>(async adminId =>
            {
                start:
                var db = new Db();
                var date = DateTime.Now.Date;
                var roles = (from role in db.userRoles
                             join roleData in db.roleData on role.id equals roleData.role_id
                             where role.user_id == adminId && roleData.quest_info != null && roleData.last_login < date && role.inited
                             select role).ToList();
                db.Dispose();
                var loginTaskActor = Context.ActorOf<LoginTaskActor>();
                foreach (var role in roles)
                {
                    db = new Db();
                    var device = db.GetDeviceById(role.device_id);
                    if (device == null)
                        continue;
                    var data = db.GetRoleDataByRoleId(role.id);
                    if (data == null)
                        continue;
                    db.Dispose();
                    await loginTaskActor.Ask<bool>(new LoginTask
                    {
                        role = role,
                        device = device,
                        roleData = data,
                        task_data = new Dictionary<string, string>(),
                        excuteType = ExcuteType.Normal,
                    });
                }
                await Task.Delay(TimeSpan.FromHours(1));
                goto start;
            });
        }
    }
}
