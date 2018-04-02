using Disunity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Core
{
    public static class AssetManagerExt
    {
        static Dictionary<string, Task> downloadLock = new Dictionary<string, Task>();

        public static async Task DownloadAsset(AssetData assetData)
        {
            var webClient = new WebClient();
            var url = AssetManage.getUrlString(assetData);
            if (downloadLock.ContainsKey(assetData.NewName))
                return;

            await webClient.DownloadFileTaskAsync(url, assetData.BinPath);
            downloadLock.Remove(assetData.NewName);
        }

        public static async Task<AssetData> GetAsset(string assetName)
        {
            if (!AssetManage.AssetList.ContainsKey(assetName))
                return null;

            var assetData = AssetManage.AssetList[assetName];
            if (downloadLock.ContainsKey(assetName))
                await downloadLock[assetName];

            if (!assetData.IsExtrated && !assetData.IsExists)
                await DownloadAsset(assetData);

            if (!assetData.IsExists)
                return null;

            if (!assetData.IsExtrated)
            {
                if (!File.Exists(assetData.ExtratPath))
                    AssetBundleExtrator.ExtratAssetBundleFile(assetData.BinPath, assetData.ExtratPath);
                AssetBundleExtrator.ExtratAssetsFile(assetData.ExtratPath);
                if (!assetData.IsExtrated)
                    return null;
            }
            return assetData;
        }
    }
}
