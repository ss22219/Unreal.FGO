using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Common.ActorParam;
using Unreal.FGO.Common.ActorResult;
using Akka.Actor;
using Unreal.FGO.Repostory.Model;
using Unreal.FGO.Repostory;
using Unreal.FGO.Common;

namespace Unreal.FGO.TaskService.TaskExcutor
{
    public class Regist : TaskExcutorBase
    {
        public Regist()
        {
            ReceiveAsync<RegistParam>(async param =>
            {
                var res = new RegistResut() { code = 0, roleId = 0 };
                try
                {
                    var db = new Db();
                    device device = null;
                    user_role role = null;
                    if (param.username != null)
                        role = db.userRoles.Where(r => r.username == param.username).FirstOrDefault();
                    if (role == null)
                    {
                        logger.Info("创建新帐号");
                        role = new user_role()
                        {
                            username = param.username,
                            password = param.password,
                            create_time = DateTime.Now,
                            last_update_time = DateTime.Now,
                            registed = true,
                            user_id = param.userId,
                            platform = param.paltform
                        };
                    }
                    else if (role.registed)
                    {
                        logger.Warn("帐号已经注册");
                        res.roleId = role.id;
                        Sender.Tell(res);
                        return;
                    }
                    else
                    {
                        logger.Info("使用已有帐号");
                        device = db.GetDeviceById(role.device_id);
                    }

                    if (device == null)
                    {
                        var presets = db.devicePresets.AsNoTracking().Where(p => p.platform_type == param.paltform).ToList();
                        if (presets.Count == 0)
                        {
                            res.code = -1;
                            res.message = "不支持该平台";
                            Sender.Tell(res);
                            return;
                        }
                        device = new device(presets.FirstOrDefault());
                        device.GenerateDeviceid();
                    }

                    db.Dispose();
                    var serverApi = InitServerApi(role, device, null);

                    logger.Info("注册开始");
                    var response = await serverApi.ApiClientRegv3(role.username, role.password);
                    if (response.code != -105)
                    {
                        res.code = -500;
                        res.message = response.message;
                        Sender.Tell(res);
                        return;
                    }
                    var bytes = await serverApi.GetCaptcha();
                    await Task.Delay(2000);
                    bytes = await serverApi.GetCaptcha();

                    if (bytes == null)
                    {
                        logger.Error("验证码获取失败");
                        res.code = -2;
                        res.message = "验证码获取失败";
                        Sender.Tell(res);
                        return;
                    }

                    var valCode = NxHelper.GetImagResult(bytes);
                    if (valCode == null)
                    {
                        logger.Error("验证码识别失败");
                        res.code = -3;
                        res.message = "验证码识别失败";
                        Sender.Tell(res);
                        return;
                    }

                    response = await serverApi.ApiClientRegv3(role.username, role.password, valCode);
                    if (response.code == 500008)
                    {
                        logger.Warn("帐号已经被注册");
                        res.roleId = role.id;
                        db = new Db();
                        role = db.userRoles.Find(role.id);
                        role.create_time = DateTime.Now;
                        role.registed = true;
                        db.SaveChanges();
                        db.Dispose();
                        Sender.Tell(res);
                        return;
                    }
                    if (response.code != 0)
                    {
                        logger.Error(response.message);
                        res.code = -4;
                        res.message = "注册失败：" + response.message;
                        Sender.Tell(res);
                        return;
                    }

                    logger.Info("注册成功");
                    db = new Db();

                    if (device.id == 0)
                    {
                        device.user_id = param.userId;
                        db.devices.Add(device);
                        db.SaveChanges();
                    }
                    role.registed = true;
                    role.device_id = device.id;
                    role.create_time = DateTime.Now;
                    if (role.id == 0)
                    {
                        db.userRoles.Add(role);
                    }
                    db.SaveChanges();
                    res.roleId = role.id;
                    db.Dispose();

                }
                catch (Exception ex)
                {
                    res.code = -500;
                    res.message = ex.ToString();
                }
                Sender.Tell(res);
            });
        }
    }
}
