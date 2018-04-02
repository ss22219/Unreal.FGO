using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Repostory.Model
{
    public class task_data
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        [Column(TypeName = "text")]
        public string value { get; set; }
        public int task_id { get; set; }
        public int user_role_id { get; set; }
    }

    public class system_info
    {
        [Key]
        public string name { get; set; }
        public string value { get; set; }
    }

    public class user
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_login_time { get; set; }
        public string token { get; internal set; }
    }

    public class user_role
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }
        [Required]
        public int device_id { get; set; }
        [Required]
        public int user_id { get; set; }
        public bool registed { get; set; }
        public bool inited { get; set; }
        public int platform { get; set; }
        public bool chaptered { get; set; }
        public DateTime? last_task_time { get; set; }
    }

    public class user_task_step
    {

    }

    public class task_log
    {
        public int id { get; set; }
        public int task_id { get; set; }
        public DateTime create_time { get; set; }
        public string message { get; set; }
        public int action { get; set; }
    }

    public class user_task
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string deckid { get; set; }
        public string follower_id { get; set; }
        public string action { get; set; }
        public int current_action { get; set; }
        public string excute_rule { get; set; }
        public int re_excute_count { get; set; }
        public int state { get; set; }
        public int error_type { get; set; }
        public bool enable { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_update_time { get; set; }

        [Required]
        public int user_role_id { get; set; }

        [Required]
        public int user_id { get; set; }
        public string quest_ids { get; set; }
        public bool useitem { get; set; }
        public DateTime expires_time { get; set; }
        public string battlId { get; set; }
    }

    public class device_preset
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// ios:1 android:3
        /// </summary>
        public int platform_type { get; set; }

        /// <summary>
        /// iOS 10.2 | Android OS 4.4.2 / API-19 (KOT49H/I9500XXUFNC1)
        /// </summary>
        public string os { get; set; }

        /// <summary>
        /// iPhone9,2 GT-I9500
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// 系统版本 10.2 4.4.2 
        /// </summary>
        public string pf_ver { get; set; }

        /// <summary>
        /// iPhone9,2  samsung GT-I9500
        /// </summary>
        public string ptype { get; set; }

        /// <summary>
        /// 1242*2208 1280*720 
        /// </summary>
        public string dp { get; set; }
    }

    public class task_error
    {
        [Key]
        public int id { get; set; }
        public int error { get; set; }
        public string message { get; set; }
        public string source_code { get; set; }
        public string source_message { get; set; }
        public string source_data { get; set; }
        public DateTime create_time { get; set; }
        public int action { get; set; }
        public int task_id { get; set; }
    }

    public class role_data
    {
        [Required]
        public int role_id { get; set; }
        public string bilibili_id { get; set; }
        public string rguid { get; set; }
        [Key]
        public int id { get; set; }
        public string usk { get; set; }
        public string access_token { get; set; }
        public long access_token_expires { get; set; }
        public string nickname { get; set; }
        public string game_user_id { get; set; }
        public string face { get; set; }
        public string s_face { get; set; }
        public string quest_info { get; set; }
        public string svt_info { get; set; }
        public string deck_info { get; set; }
        public string user_game { get; set; }
        public int stone { get; set; }
        public DateTime last_login { get; set; }
        public string user_item { get; set; }
        public string cookie { get; set; }
        public string battle_id { get; set; }
    }

    public class device
    {
        public device()
        {

        }
        public device(device_preset preset)
        {
            platform_type = preset.platform_type;
            os = preset.os;
            model = preset.model;
            pf_ver = preset.pf_ver;
            ptype = preset.ptype;
            dp = preset.dp;
        }

        [Required]
        public int user_id { get; set; }
        public string idfa { get; set; }
        public string udid { get; set; }
        public string deviceid { get; set; }
        [Key]
        public int id { get; set; }
        /// <summary>
        /// ios:1 android:3
        /// </summary>
        public int platform_type { get; set; }

        /// <summary>
        /// iOS 10.2 | Android OS 4.4.2 / API-19 (KOT49H/I9500XXUFNC1)
        /// </summary>
        public string os { get; set; }

        /// <summary>
        /// iPhone9,2 GT-I9500
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// 系统版本 10.24.4.2 
        /// </summary>
        public string pf_ver { get; set; }

        /// <summary>
        /// iPhone9,2  samsung GT-I9500
        /// </summary>
        public string ptype { get; set; }

        /// <summary>
        /// 1242*2208 1280*720 
        /// </summary>
        public string dp { get; set; }
    }
}
