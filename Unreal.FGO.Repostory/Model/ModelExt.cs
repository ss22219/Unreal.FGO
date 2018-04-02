
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Repostory.Model
{
    public static class ModelExt
    {

        public static void GenerateToken(this user user)
        {
            //79-125 a-z 65-90 A-Z 48-57 0-9
            var random = new Random();
            user.token = "";
            for (int i = 0; i < 20; i++)
            {
                switch (random.Next(0, 3))
                {
                    case 0:
                        user.token += (char)random.Next(79, 126);
                        break;
                    case 1:
                        user.token += (char)random.Next(48, 56);
                        break;
                    case 2:
                        user.token += (char)random.Next(65, 91);
                        break;
                    default:
                        break;
                }
            }
            if (new Db().GetUserByToken(user.token) != null)
                GenerateToken(user);
        }

        public static void GenerateDeviceid(this device device)
        {
            device.idfa = Guid.NewGuid().ToString().ToUpper();
            device.udid = Guid.NewGuid().ToString().ToUpper();
            device.deviceid = Guid.NewGuid().ToString().ToUpper();
        }
    }
}
