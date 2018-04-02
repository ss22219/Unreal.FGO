using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(Unreal.FGO.WebApi.Startup))]

namespace Unreal.FGO.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map =>
            {
                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = true//跨域的关键语句
                };
                map.RunSignalR(hubConfiguration);
            });
            //app.UseWebApi()
            //IAuthenticationManager authenticationManager = GetAuthenticationManagerOrThrow(request);
        }
    }
}
