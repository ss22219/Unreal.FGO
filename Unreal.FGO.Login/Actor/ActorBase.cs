using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Console.Helper;
using Unreal.FGO.Core.Api;
using Unreal.FGO.Repostory;

namespace Unreal.FGO.Console.Actor
{
    public class TaskErrorInfo
    {
        public GameError error { get; set; }
        public GameAction action { get; set; }
        public string message { get; set; }
        public int taskId { get; set; }
        public string sourceCode { get; set; }
        public string sourceMessage { get; set; }
        public string sourceData { get; set; }
    }
    public class ActorBase : ReceiveActor
    {
        protected async Task<bool> TaskError(TaskErrorInfo info, BaseResponse response = null, string message = null)
        {
            if (message != null)
                info.message = message;
            if (response != null)
            {
                info.sourceCode = response.code.ToString();
                info.sourceData = response.RequestMessage;
                info.sourceMessage = response.message;
                if (response.message != null && response.message.IndexOf("维护") != -1)
                {
                    info.error = GameError.MAINTAIN;
                }
                else if (response.code == 99)
                {
                    info.error = GameError.NET_ERROR;
                }
            }
            return await Context.ActorOf<TaskErrorActor>("TaskError").Ask<bool>(info);
        }

        protected async Task TaskErrorAndBack(int taskId, GameAction action, BaseResponse response = null, string message = null)
        {
            if (message == null)
                message = action.ToString() + " Error";
            var info = new TaskErrorInfo()
            {
                action = action,
                error = (GameError)(-(int)action),
                taskId = taskId
            };
            Sender.Tell(await TaskError(info, response, message));
        }

        protected Task<bool> TaskError(int taskId, GameAction action, BaseResponse response = null, string message = null)
        {
            if (message == null)
                message = action.ToString() + " Error";
            var info = new TaskErrorInfo()
            {
                action = action,
                error = (GameError)(-1 * (int)action),
                taskId = taskId
            };
            return TaskError(info, response, message);
        }
    }
}
