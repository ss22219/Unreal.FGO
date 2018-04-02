using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unreal.FGO.Repostory.Model;

namespace Unreal.FGO.Repostory
{
    public class Db : DbContext
    {
        public Db() : base("name=DefaultConnection")
        {
        }

        public void Add<T>(T e) where T : class
        {
            this.Set<T>().Add(e);
            this.Entry<T>(e).State = EntityState.Added;
            this.SaveChanges();
        }

        public void Update<T>(T e) where T : class
        {

            this.Set<T>().Attach(e);
            this.Entry<T>(e).State = EntityState.Modified;
            this.SaveChanges();
        }

        public List<user_role> GetUserRoleByUserId(int id)
        {
            return userRoles.Where(ur => ur.user_id == id).AsNoTracking().ToList();
        }

        public List<device> GetDeviceByUserId(int id)
        {
            return devices.Where(d => d.user_id == id).AsNoTracking().ToList();
        }

        public user GetUserById(int userId)
        {
            return users.Where(user => user.id == userId).AsNoTracking().FirstOrDefault();
        }

        public device GetDeviceById(int id)
        {
            return devices.Where(e => e.id == id).AsNoTracking().FirstOrDefault();
        }

        public device_preset GetDevicePresetById(int id)
        {
            return devicePresets.Where(e => e.id == id).AsNoTracking().FirstOrDefault();
        }

        public user_role GetUserRoleById(int id)
        {
            return userRoles.Where(e => e.id == id).AsNoTracking().FirstOrDefault();
        }

        public void AddTaskLog(task_log task_log)
        {
            taskLog.Add(task_log);
            SaveChanges();
        }

        public List<user_task> GetTaskByUserId(int id)
        {
            return userTasks.Where(e => e.user_id == id).AsNoTracking().ToList();
        }

        public List<user_task> GetTaskByRoleId(int id)
        {
            return userTasks.Where(e => e.user_role_id == id).AsNoTracking().ToList();
        }

        public system_info GetSystemInfoById(string name)
        {
            return systemInfos.Where(e => e.name == name).AsNoTracking().FirstOrDefault();
        }

        public user_task GetTaskById(int id)
        {
            return userTasks.Where(e => e.id == id).AsNoTracking().FirstOrDefault();
        }
        public DbSet<user> users { get; set; }
        public DbSet<task_data> taskDatas { get; set; }

        public List<task_data> GetTaskDataByTaskId(int id)
        {
            return taskDatas.Where(e => e.task_id == id).AsNoTracking().ToList();
        }

        public user GetUserByToken(string token)
        {
            return users.Where(u => u.token == token).AsNoTracking().FirstOrDefault();
        }

        public DbSet<device> devices { get; set; }

        public role_data GetRoleDataByRoleId(int roleId)
        {
            return roleData.Where(e => e.role_id == roleId).AsNoTracking().FirstOrDefault();
        }

        public DbSet<task_log> taskLog { get; set; }
        public DbSet<task_error> taskError { get; set; }
        public DbSet<role_data> roleData { get; set; }
        public DbSet<device_preset> devicePresets { get; set; }
        public DbSet<user_role> userRoles { get; set; }
        public DbSet<user_task> userTasks { get; set; }
        public DbSet<system_info> systemInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public static string MD5(string str)
        {
            str += "Unreal.FGO";
            byte[] data = Encoding.UTF8.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToLower();
        }

        public user Login(string username, string password)
        {
            password = password.Length == 32 ? password : MD5(password);
            var user = users.Where(u => u.username == username && u.password == password).AsNoTracking().FirstOrDefault();
            if (user != null)
            {
                user.GenerateToken();
                user.last_login_time = DateTime.Now;
                Update(user);
            }
            return user;
        }
    }
}
