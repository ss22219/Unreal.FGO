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
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory;
using Unreal.FGO.Repostory.Model;


namespace Unreal.FGO.WebApi.Controllers
{

    public abstract class ApiBase : ApiController
    {
        protected user user;
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            if (controllerContext.Controller is UserController)
                return base.ExecuteAsync(controllerContext, cancellationToken);
            string token = string.Empty;
            IEnumerable<string> headers = null;
            if (controllerContext.Request.Headers.Contains("Authorization"))
                headers = controllerContext.Request.Headers.GetValues("Authorization");
            else if (controllerContext.Request.Headers.Contains("token"))
                headers = controllerContext.Request.Headers.GetValues("token");
            else if (controllerContext.Request.Headers.Contains("Access-Control-Request-Headers"))
                headers = controllerContext.Request.Headers.GetValues("Access-Control-Request-Headers");
            if (headers != null && headers.Any())
                token = headers.First();
            else
            {
                var dic = controllerContext.Request.GetQueryNameValuePairs().Where(d => d.Key == "token");
                if (dic.Any())
                    token = dic.First().Value;
            }
            user = db.GetUserByToken(token);
            if (user != null)
                return base.ExecuteAsync(controllerContext, cancellationToken);

            var content = new StringContent(JsonConvert.SerializeObject(new JsonModel()
            {
                code = 88,
                message = "请登陆后继续操作"
            }));

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var message = new HttpResponseMessage()
            {
                Content = content
            };
            return Task.FromResult(message);

        }
        protected Db db = new Db();
        protected JsonResult<JsonModel<T>> Json<T>(int code, T data, string message)
        {

            return base.Json<JsonModel<T>>(new JsonModel<T>()
            {
                message = message,
                code = code,
                data = data
            });
        }
        protected new JsonResult<JsonModel<T>> Json<T>(T data)
        {

            return base.Json<JsonModel<T>>(new JsonModel<T>()
            {
                code = 0,
                data = data
            }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings, System.Text.Encoding.UTF8);
        }

        protected JsonResult<JsonModel> Json(int code, string message)
        {

            return base.Json<JsonModel>(new JsonModel()
            {
                code = code,
                message = message
            }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings, System.Text.Encoding.UTF8);
        }

        protected JsonResult<JsonModel> InitDatabase()
        {
            var version = db.systemInfos.Find("dataVer");
            if (version == null)
                return Json(-1, "找不到版本配置信息");
            if (AssetManage.Database == null)
                AssetManage.LoadDatabase(version.value);
            return Json(0, null);
        }
    }
}