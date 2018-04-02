using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Unreal.FGO.Core;
using Unreal.FGO.Repostory;
using WebApiContrib.Formatting.Jsonp;

namespace Unreal.FGO.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                var db = new Db();
                db.systemInfos.ToList();
                db.Dispose();
            }
            catch (Exception)
            {
                Process.GetCurrentProcess().Kill();
            }
            GlobalConfiguration.Configuration.AddJsonpFormatter();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AssetManage.DataPath = Server.MapPath("~/") + "/";
            AkkaSystem.Init();
        }

        protected void Application_PreSendRequestHeaders()
        {
            this.Response.Headers.Remove("Server");
            this.Response.Headers.Remove("X-AspNet-Version");
        }

        protected void Application_Error()
        {   
            //获得最后一个Exception
            Exception ex = this.Context.Server.GetLastError();
            File.WriteAllText(Server.MapPath("~/")  + "/error.log", ex.ToString());
            this.Context.Server.ClearError();
        }
    }
}
