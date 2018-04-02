using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
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
using Unreal.FGO.Core.Api;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.WebApi.Controllers
{
    public class JsonModel
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public class MapData
    {
        public List<mstEvent> events { get; set; }
        //public List<mstEventQuest> eventQuests { get; set; }
        public List<War> wars { get; set; }
        public List<mstSvtGroup> svtGroups { get; internal set; }
        //public List<mstMission> messions { get; internal set; }
        public List<mstQuestGroup> questGroups { get; internal set; }
    }

    public class War : mstWar
    {
        public List<Spot> spots { get; set; }
        public List<SpotRoad> spotRoads { get; set; }
    }

    public class SpotRoad : mstSpotRoad
    {
    }

    public class Quest : mstQuest
    {
        public List<mstQuestPhase> phases { get; set; }
        public List<mstQuestRelease> releases { get; set; }
    }
    public class Spot : mstSpot
    {
        public List<Quest> quests { get; set; }
    }

    public class JsonModel<T> : JsonModel
    {
        public T data { get; set; }
    }

    public class UserQuest
    {
        public string id { get; set; }
        public int phase { get; set; }
    }

    public class UserController : ApiBase
    {

        [HttpGet]
        [HttpPost]
        [HttpOptions]
        public IHttpActionResult getMapData(string callback = "")
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~/");
            var version = db.systemInfos.Find("dataVer");
            if (version == null)
            {
                return Json(-1, "找不到版本配置信息");
            }
            if (!File.Exists(root + "map_" + version.value + ".json"))
            {
                AssetManage.LoadDatabase(version.value);
                var map = new MapData();
                //map.eventQuests = AssetManage.Database.mstEventQuest;
                map.events = AssetManage.Database.mstEvent;
                map.questGroups = AssetManage.Database.mstQuestGroup;
                //map.messions = AssetManage.Database.mstMission;
                map.svtGroups = AssetManage.Database.mstSvtGroup;
                map.wars = new List<War>();
                Mapper.Initialize((config) =>
                {
                    config.CreateMap<mstWar, War>();
                    config.CreateMap<mstQuest, Quest>();
                    config.CreateMap<mstSpot, Spot>();
                    config.CreateMap<mstSpotRoad, SpotRoad>();
                });
                AutoMapper.Mapper.Map(AssetManage.Database.mstWar, map.wars);
                foreach (var war in map.wars)
                {
                    war.spots = new List<Spot>();
                    foreach (var spot in AssetManage.Database.mstSpot)
                    {
                        if (spot.warId == war.id)
                        {
                            var nspot = Mapper.Map<mstSpot, Spot>(spot);
                            war.spots.Add(nspot);
                            nspot.quests = new List<Quest>();
                            foreach (var quest in AssetManage.Database.mstQuest)
                            {
                                if (quest.spotId == spot.id)
                                {
                                    var nquest = Mapper.Map<mstQuest, Quest>(quest);
                                    nspot.quests.Add(nquest);
                                    nquest.phases = new List<mstQuestPhase>();
                                    nquest.releases = new List<mstQuestRelease>();
                                    foreach (var phase in AssetManage.Database.mstQuestPhase)
                                    {
                                        if (phase.questId == quest.id)
                                            nquest.phases.Add(phase);
                                    }
                                    foreach (var release in AssetManage.Database.mstQuestRelease)
                                    {
                                        if (release.questId == quest.id)
                                            nquest.releases.Add(release);
                                    }
                                }
                            }
                        }
                    }
                    war.spotRoads = new List<SpotRoad>();
                    foreach (var spotRoad in AssetManage.Database.mstSpotRoad)
                    {
                        if (spotRoad.warId == war.id)
                            war.spotRoads.Add(Mapper.Map<mstSpotRoad, SpotRoad>(spotRoad));
                    }
                }
                File.WriteAllText(root + "map_" + version.value + ".json", JsonConvert.SerializeObject(map));
                QiniuHelper.Upload("map_" + version.value + ".json", root + "map_" + version.value + ".json");
            }
            var url = "http://oj3vd47gp.bkt.clouddn.com/map_" + version.value + ".json";
            return Redirect(url);
        }



        [HttpOptions]
        public IHttpActionResult Login()
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Login([FromBody]user user)
        {
            user = db.Login(user.username, user.password);
            if (user != null)
            {
                return Json(user.token);
            }
            return Json(-1, "用户名或密码不正确");
        }

        [HttpPost]
        public IHttpActionResult Regist([FromBody]user user)
        {
            if (db.users.Where(u => u.username == user.username).Any())
                return Json(-1, "该用户已经存在");
            if (string.IsNullOrEmpty(user.username) || user.username.Length < 6 || user.username.Length > 16)
                return Json(-1, "用户名长度不正确");
            if (string.IsNullOrEmpty(user.password) || user.password.Length < 6 || user.password.Length > 16)
                return Json(-1, "密码长度不正确");

            user = new user() { username = user.username, password = Db.MD5(user.password), last_login_time = DateTime.Now, create_time = DateTime.Now };
            user.GenerateToken();
            db.Add(user);
            if (user != null || user.id != 0)
            {
                return Json(user.token);
            }
            return Json(-2, "系统未知错误");
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
}