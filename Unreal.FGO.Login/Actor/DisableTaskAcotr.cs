using Unreal.FGO.Console.Helper;
using Akka.Actor;
namespace Unreal.FGO.Console.Actor
{
    public class DisableTaskAcotr : ActorBase
    {
        public DisableTaskAcotr()
        {
            Receive<int>(id =>
            {
                var db = DbHelper.DB;
                var task = db.GetTaskById(id);
                if (task != null)
                {
                    task.enable = false;
                    db.SaveChanges();
                }
                Sender.Tell(true);
            });
        }
    }
}