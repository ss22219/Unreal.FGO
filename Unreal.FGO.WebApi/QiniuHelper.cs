using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unreal.FGO.WebApi
{
    public class QiniuHelper
    {
        public static void Upload(string name, string local)
        {
            string AK = "xWCA_xrd2Q_dbsIbruOY1-hBlAcJyKRgnTcqiNNV";
            string SK = "zJaTqWDc1Fpt79_xSHQp1LTg7IPDL2SgzRTmhvjx";
            // 目标空间名
            string bucket = "unreal";
            // 目标文件名
            string saveKey = name;
            // 本地文件
            string localFile = local;

            // 上传策略
            PutPolicy putPolicy = new PutPolicy();
            // 设置要上传的目标空间
            putPolicy.Scope = bucket;
            // 上传策略的过期时间(单位:秒)
            putPolicy.SetExpires(360000);
            // 文件上传完毕后，在多少天后自动被删除
            putPolicy.DeleteAfterDays = 360;

            // 请注意这里的Zone设置(如果不设置，就默认为华东机房)
            // var zoneId = Qiniu.Common.AutoZone.Query(AK,BUCKET);
            Qiniu.Common.Config.ConfigZone(Qiniu.Common.ZoneID.CN_South);

            Mac mac = new Mac(AK, SK); // Use AK & SK here
                                       // 生成上传凭证
            string uploadToken = Auth.createUploadToken(putPolicy, mac);

            UploadOptions uploadOptions = null;
            // 方式1：使用UploadManager
            //默认设置 Qiniu.Common.Config.PUT_THRESHOLD = 512*1024;
            //可以适当修改,UploadManager会根据这个阈值自动选择是否使用分片(Resumable)上传  
            UploadManager um = new UploadManager();
            um.uploadFile(localFile, saveKey, uploadToken, uploadOptions, null);
        }
    }
}