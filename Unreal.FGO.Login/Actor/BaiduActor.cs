using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Console.Actor
{
    public class BaiduActor : ActorBase
    {
        public BaiduActor()
        {
            ReceiveAnyAsync(async (o) =>
            {
                System.Console.WriteLine("Task.CurrentId:" + Task.CurrentId);
                var response = await new HttpClient().GetAsync("https://www.baidu.com/");
                System.Console.WriteLine("Task.CurrentId:" + Task.CurrentId);
                System.Console.WriteLine(response.StatusCode);
                var str = await response.Content.ReadAsStringAsync();
                Context.Sender.Tell(new LoginTask(), Context.Self);
            });
        }
    }
}
