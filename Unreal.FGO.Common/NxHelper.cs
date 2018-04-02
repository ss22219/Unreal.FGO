using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Common
{
    public class NxHelper
    {
        public static string GetImagResult(byte[] imageBytes)
        {
            try
            {
                var result = new WebClient().UploadData("http://api.nxdati.com/?nx_api=nx_sbi&nx_user=ss22219&nx_pwd=123456&nx_type=3050", imageBytes);
                var content = JsonConvert.DeserializeObject<NxResult>(Encoding.UTF8.GetString(result));
                return content.nx_code != "0" ? content.nx_result : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class NxResult
        {
            public string nx_code { get; set; }
            public string nx_result { get; set; }
        }
    }
}
