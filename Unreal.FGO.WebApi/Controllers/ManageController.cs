using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;
using Akka.Actor;
using Unreal.FGO.Common.ActorResult;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Core.Api;
using AutoMapper;

namespace Unreal.FGO.WebApi.Controllers
{
    public class ManageController : ApiBase
    {
        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public async Task<IHttpActionResult> questList(int roleId)
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            var roleData = db.GetRoleDataByRoleId(roleId);
            if (roleData == null || string.IsNullOrEmpty(roleData.quest_info))
            {
                var role = db.GetUserRoleById(roleId);
                if (role == null)
                    return Json(-1, "帐号不存在");
                var device = db.GetDeviceById(role.device_id);
                if (device == null)
                    return Json(-1, "帐号平台信息不存在");
                var result = await Login(role, device);
                if (result.Content.code == 0)
                    roleData = db.GetRoleDataByRoleId(roleId);
                else
                    return result;
            }
            if (roleData == null || string.IsNullOrEmpty(roleData.quest_info))
                return Json(-1, "获取副本信息失败，该账号可能没有创建角色");

            var questChecker = new QuestChecker()
            {
                TimetampSecond = new ServerApi().TimetampSecond,
                userQuest = JsonConvert.DeserializeObject<List<HomeUserquest>>(roleData.quest_info)
            };
            var list = questChecker.GetUserQuestList();
            var userQuests = list.Select(q =>
            {
                var uq = questChecker.userQuest.FirstOrDefault(q2 => q2.questId == q.id);
                return new UserQuest() { id = q.id, phase = uq != null ? uq.questPhase : 0 };
            }).ToList();
            return Json(userQuests);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult taskList()
        {
            var tasks = db.GetTaskByUserId(user.id);
            var taskLogs = new Dictionary<int, task_log>();
            foreach (var task in tasks)
            {
                var log = db.taskLog.AsNoTracking().Where(l => l.task_id == task.id).OrderByDescending(l => l.create_time).FirstOrDefault();
                if (log != null)
                    taskLogs[task.id] = log;
            }
            return Json(new { tasks = tasks, lastLogs = taskLogs });
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult userInfo(string callback = "")
        {
            var userRoles = db.GetUserRoleByUserId(user.id);
            var device = db.GetDeviceByUserId(user.id);
            var devicePresets = db.devicePresets.ToList();
            var tasks = db.GetTaskByUserId(user.id);
            List<role_data> userRoleDatas = new List<role_data>();
            foreach (var role in userRoles)
            {
                userRoleDatas.Add(db.GetRoleDataByRoleId(role.id));
            }
            Mapper.Initialize((config) =>
            {
                config.CreateMap<user_role, RoleMode>();
            });
            var roles = new List<RoleMode>();
            foreach (var role in userRoles)
            {
                var r = Mapper.Map<user_role, RoleMode>(role);
                var data = userRoleDatas.FirstOrDefault(d => d.role_id == r.id);
                if (data != null)
                {
                    r.stone = data.stone;
                }
                roles.Add(r);
            }
            return Json<UserInfoModel>(new UserInfoModel()
            {
                roles = roles,
                devicePresets = devicePresets,
                devices = device,
                battles = userRoleDatas.Select(u => string.IsNullOrEmpty(u.battle_id) ? null : new battleData()
                {
                    battleId = u.battle_id,
                    roleId = u.role_id
                }).ToList(),
                tasks = tasks
            });
        }

        public IHttpActionResult AddDevice([FromBody]device device)
        {
            device.GenerateDeviceid();
            device.user_id = user.id;
            db.Add(device);
            return Json(device);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public async Task<IHttpActionResult> battleSetup([FromBody] BattSetupReq req)
        {
            var result = await AkkaSystem.BattleSetupActor().Ask<BattleSetupResult>(new BattleSetupParam()
            {
                questId = req.questId,
                roleId = req.roleId
            });
            if (result.code != 0)
                return Json(result.code, result.message);
            else
                return Json(result);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public async Task<IHttpActionResult> battleResult([FromBody] BattResultReq req)
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            var result = await AkkaSystem.BattleResultActor().Ask<BattleResultResult>(new BattleResultParam()
            {
                battleId = req.battleId,
                roleId = req.roleId
            });
            return Json(result.code, result.message);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult quests()
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            return Json(AssetManage.Database.mstQuest.Select(q => new { name = q.name, id = q.id }));
        }


        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult svts()
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            return Json(AssetManage.Database.mstSvt.Where(s => s.type == "1").Select(q => new { name = q.name, id = q.id }));
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public async Task<IHttpActionResult> Role([FromBody]RoleModel roleInfo)
        {
            var role = db.userRoles.Find(roleInfo.id);
            if (role != null && role.user_id != user.id)
                return Json(0, "");
            device device = null;
            if (role == null)
            {
                role = new user_role()
                {
                    create_time = DateTime.Now,
                    last_update_time = DateTime.Now,
                    password = roleInfo.password,
                    username = roleInfo.username,
                    user_id = user.id,
                    platform = roleInfo.platform
                };
                var devicePreset = db.devicePresets.Where(d => d.platform_type == roleInfo.platform).AsNoTracking().FirstOrDefault();
                if (devicePreset == null)
                    return Json(-1, "不支持该平台用户");

                device = new device(devicePreset);
                device.user_id = user.id;
                device.GenerateDeviceid();
                db.Add(device);
                db.SaveChanges();
                role.device_id = device.id;
                if (user.username == "super_admin")
                {
                    role.create_time = new DateTime(1990, 1, 1);
                    role.last_update_time = new DateTime(1990, 1, 1);
                    db.Add(role);
                    db.SaveChanges();
                    return Json(0, "success");
                }
                db.Add(role);
                db.SaveChanges();
            }
            else
            {
                device = db.devices.Find(role.device_id);
                if (device == null || device.platform_type != roleInfo.platform)
                {
                    var roleData = db.roleData.Where(r => r.role_id == role.id).FirstOrDefault();
                    db.roleData.Remove(roleData);
                    if (device != null)
                        db.devices.Remove(device);
                    var devicePreset = db.devicePresets.Where(d => d.platform_type == roleInfo.platform).AsNoTracking().FirstOrDefault();
                    if (devicePreset == null)
                        return Json(-1, "不支持该平台用户");
                    device = new device(devicePreset);
                    device.user_id = user.id;
                    device.GenerateDeviceid();
                    db.Add(device);
                    db.SaveChanges();
                    role.device_id = device.id;
                }
                role.username = roleInfo.username;
                role.password = roleInfo.password;
                role.last_update_time = DateTime.Now;
                db.SaveChanges();
            }
            return await Login(role, device);
        }

        private async Task<JsonResult<JsonModel>> Login(user_role role, device device)
        {
            var loginActor = AkkaSystem.LoginActor();
            if (loginActor != null)
            {
                var result = await loginActor.Ask<LoginResult>(new LoginParam()
                {
                    device = device,
                    role = role,
                    home = true
                }, TimeSpan.FromMinutes(1));
                if (result == null)
                    return Json(-1, "登陆失败，内部执行出错");
                return Json(result.code, result.message);
            }
            return Json(-500, "loginActor is null");
        }
        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult delRole(int id)
        {
            var role = db.userRoles.Find(id);
            if (role != null && role.user_id == user.id)
            {
                var roleDatas = db.roleData.Where(d => d.role_id == id);
                var device = db.devices.Where(d => d.id == role.device_id);
                var tasks = db.userTasks.Where(t => t.user_role_id == id);
                var taskDatas = db.taskDatas.Where(t => t.user_role_id == id);
                db.roleData.RemoveRange(roleDatas);
                db.devices.RemoveRange(device);
                db.taskDatas.RemoveRange(taskDatas);
                db.userTasks.RemoveRange(tasks);
                db.userRoles.Remove(role);
                db.SaveChanges();
            }
            return Json(0, "success");
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult delTask(int id)
        {
            var task = db.userTasks.Find(id);
            if (task != null && task.user_id == user.id)
            {
                var taskDatas = db.taskDatas.Where(t => t.task_id == id);
                db.taskDatas.RemoveRange(taskDatas);
                db.userTasks.Remove(task);
                db.SaveChanges();
            }
            return Json(0, "success");
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult TaskLog(int id)
        {
            var logs = db.taskLog.AsNoTracking().Where(t => t.task_id == id).OrderByDescending(t => t.create_time).Take(20).ToList();
            return Json(logs);
        }
        protected static DateTime dtUnixEpoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public long TimetampSecond
        {
            get
            {
                return ((long)DateTime.Now.Subtract(dtUnixEpoch).TotalSeconds); ;
            }
        }


        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult RoleSvts(int roleId)
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            var userRole = db.GetUserRoleById(roleId);
            if (userRole == null || userRole.user_id != user.id)
                return Json(-1, "帐号不存在");
            var roleData = db.GetRoleDataByRoleId(roleId);
            if (roleData == null || roleData.svt_info == null)
            {
                return Json(-2, "还没有登陆过该帐号");
            }
            var list = new List<SvtModel>();
            var svts = JsonConvert.DeserializeObject<List<HomeUsersvt>>(roleData.svt_info);
            foreach (var svt in svts)
            {
                var model = new SvtModel()
                {
                    id = svt.id,
                    svtId = svt.svtId,
                    lv = svt.lv,
                    limit = svt.limitCount,
                };
                var mstSvt = AssetManage.Database.mstSvt.FirstOrDefault(s => svt.svtId == s.id && s.type == "1");
                if (mstSvt == null)
                    continue;
                model.name = mstSvt.name;

                var limit = AssetManage.Database.mstSvtLimit.FirstOrDefault(l => l.svtId == svt.svtId && l.limitCount == svt.limitCount);
                if (limit != null)
                    model.rarity = limit.rarity;
                list.Add(model);
            }
            list = list.OrderByDescending(l => l.rarity).ToList();
            return Json(list);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult Items(int roleId)
        {
            var res = InitDatabase();
            if (res.Content.code != 0)
                return res;

            var userRole = db.GetUserRoleById(roleId);
            if (userRole == null || userRole.user_id != user.id)
                return Json(-1, "帐号不存在");
            var roleData = db.GetRoleDataByRoleId(roleId);
            if (roleData == null || roleData.user_item == null)
            {
                return Json(-2, "还没有登陆过该帐号");
            }
            var list = new List<ItemModel>();
            var userItems = JsonConvert.DeserializeObject<List<HomeUseritem>>(roleData.user_item);
            var eventList = new List<ItemModel>();
            foreach (var item in userItems)
            {
                var mstItem = AssetManage.Database.mstItem.FirstOrDefault(i => i.id == item.itemId);
                if (mstItem != null && mstItem.endedAt > TimetampSecond)
                {
                    var model = new ItemModel()
                    {
                        id = mstItem.id,
                        count = item.num,
                        name = mstItem.name,
                        type = mstItem.type,
                        detail = mstItem.detail
                    };
                    if (mstItem.type == "15")
                        eventList.Add(model);
                    else
                        list.Add(model);
                }
            }
            list.InsertRange(0, eventList);
            return Json(list);
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult ResetTask(int id)
        {
            var task = db.userTasks.Find(id);
            var userRole = db.GetUserRoleById(task.user_role_id);
            if (userRole == null || userRole.user_id != user.id)
                return Json(-1, "任务不存在");
            task.start_time = null;
            task.end_time = null;
            task.state = 0;
            task.error_type = 0;
            task.re_excute_count = 0;
            task.last_update_time = DateTime.Now;
            task.expires_time = DateTime.Now;
            db.SaveChanges();
            return Json(0, "success");
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult DisableTask(int id)
        {
            var task = db.userTasks.Find(id);
            var userRole = db.GetUserRoleById(task.user_role_id);
            if (userRole == null || userRole.user_id != user.id)
                return Json(-1, "任务不存在");
            task.enable = !task.enable;
            db.SaveChanges();
            return Json(0, "success");
        }

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult Task([FromBody]user_task taskInfo)
        {
            var task = db.userTasks.Where(t => t.user_role_id == taskInfo.user_role_id).FirstOrDefault();
            var userRole = db.GetUserRoleById(taskInfo.user_role_id);
            if (userRole == null || userRole.user_id != user.id)
                return Json(-1, "帐号不存在");
            if (task == null)
            {
                try
                {
                    task = new user_task();
                    task.name = userRole.username;
                    task.create_time = DateTime.Now;
                    task.enable = true;
                    task.last_update_time = new DateTime(1990, 1, 1);
                    task.user_id = user.id;
                    task.user_role_id = taskInfo.user_role_id;
                    task.useitem = taskInfo.useitem;
                    task.action = taskInfo.action;
                    task.deckid = taskInfo.deckid;
                    task.expires_time = DateTime.Now;
                    task.quest_ids = taskInfo.quest_ids;
                    task.name = taskInfo.name;
                    task.state = 0;
                    db.userTasks.Add(task);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                task.name = userRole.username;
                task.user_id = user.id;
                task.user_role_id = taskInfo.user_role_id;
                task.useitem = taskInfo.useitem;
                task.action = taskInfo.action;
                task.deckid = taskInfo.deckid;
                task.follower_id = taskInfo.follower_id;
                task.quest_ids = taskInfo.quest_ids;
                if (task.action != "Login")
                    task.start_time = DateTime.Now;
                db.SaveChanges();
            }
            return Json(0, "success");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class UserInfoModel
    {
        public List<RoleMode> roles { get; set; }
        public List<device> devices { get; set; }
        public List<device_preset> devicePresets { get; set; }
        public List<battleData> battles { get; set; }
        public List<user_task> tasks { get; set; }
    }
    public class RoleMode : user_role
    {
        public int stone { get; set; }
    }

    public class battleData
    {
        public int roleId { get; set; }
        public string battleId { get; set; }
    }

    public class BattSetupReq
    {
        public int roleId { get; set; }
        public string questId { get; set; }
    }
    public class BattResultReq
    {
        public int roleId { get; set; }
        public string questId { get; set; }
        public string battleId { get; set; }
    }


    public class SvtModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string lv { get; set; }
        public string limit { get; internal set; }
        public string svtId { get; internal set; }
        public int rarity { get; internal set; }
    }

    public class ItemModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string detail { get; internal set; }
        public string type { get; internal set; }
    }
    public class AddRoleResult : user_role
    {
        public int platform_type { get; set; }
    }
    public class RoleModel
    {
        public int id { get; set; }
        public int platform { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}