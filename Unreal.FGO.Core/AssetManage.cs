
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Core
{
    public static class AssetManage
    {

        public static string AssetPath = @"C:\Users\Administrator\Desktop\Data\";
        public static string DataPath = "";
        public static string AssetListTxtFileName = "AssetStorage.txt";
        public static Dictionary<string, AssetData> AssetList;
        public static Database Database;
        public static string dataVer = "46";
        public static string DataServerAddress = "https://line1.patch.fate.biligame.net/bili/NewResources/Android/";
        public static string DataJsonUrl = "https://line2.patch.fate.biligame.net/1140/MasterDataCachesOutput/{dataVer}/data.txt";
        public static string AssetStorageUrl = "http://line1.s1.ios.fate.biligame.net/rongame_beta/rgfate/60_member/network/AssetStorage.txt";
        static AssetManage()
        {
            ServicePointManager.ServerCertificateValidationCallback
                       += RemoteCertificateValidate;
        }
        private static bool RemoteCertificateValidate(
           object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            // trust any certificate!!!
            return true;
        }
        public static void LoadDatabase(string version = null)
        {
            if (version != null)
                dataVer = version;
            if (!File.Exists(DataPath + "data_" + dataVer + ".json"))
            {
                var data = new WebClient().DownloadString(DataJsonUrl.Replace("{dataVer}", dataVer));
                File.WriteAllText(DataPath + "data_" + dataVer + ".json", CryptData.Decrypt(data, true));
            }
            var content = File.ReadAllText(DataPath + "data_" + dataVer + ".json");
            Database = JsonConvert.DeserializeObject<Database>(content);
        }

        public static void LoadAssetList()
        {
            if (!File.Exists(AssetPath + AssetListTxtFileName))
            {
                var data = System.Web.HttpUtility.UrlDecode(new WebClient().DownloadString(AssetStorageUrl));
                File.WriteAllText(AssetPath + AssetListTxtFileName, System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(data))));
            }

            var list = new Dictionary<string, AssetData>();
            var loadData = File.ReadAllText(AssetPath + AssetListTxtFileName);
            loadData = CryptData.TextDecrypt(loadData);
            var listData = loadData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < listData.Length; i++)
            {
                var lineData = listData[i].Split(new char[] { ',' });
                if (lineData.Length != 5)
                    continue;

                var version = int.Parse(lineData[0].Trim());
                var attrib = lineData[1];
                var size = int.Parse(lineData[2].Trim());
                var crc = uint.Parse(lineData[3].Trim());
                var name = lineData[4];
                var newname = string.Empty;
                if (lineData[4].Contains("%"))
                {
                    char[] chArray10 = new char[] { '%' };
                    char[] chArray11 = new char[] { '%' };
                    newname = lineData[4].Split(chArray10)[0] + lineData[4].Split(chArray11)[2];
                }
                else
                {
                    newname = lineData[4];
                }
                var assetInfo = new AssetData();
                assetInfo.NewName = name;
                assetInfo.Name = newname;
                assetInfo.SetUpdateInfo(version, attrib, size, crc);
                if (!list.ContainsKey(assetInfo.Name))
                    list.Add(assetInfo.Name, assetInfo);
            }
            AssetList = list;
        }

        public static string getShaName(string name)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = new UTF8Encoding().GetBytes(name);
            byte[] buffer2 = sha.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte num in buffer2)
            {
                builder.AppendFormat("{0,0:x2}", num ^ 170);
            }
            builder.Append(".bin");
            return builder.ToString();
        }

        public static string getBinName(string name)
        {
            name = name.Replace('/', '@') + ".unity3d";
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = new UTF8Encoding().GetBytes(name);
            byte[] buffer2 = sha.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (byte num in buffer2)
            {
                builder.AppendFormat("{0,0:x2}", num ^ 170);
            }
            builder.Append(".bin");
            return builder.ToString();
        }

        public static string getUrlString(AssetData assetData)
        {
            string str2;
            string str = assetData.NewName.Replace('/', '@');
            if (assetData.GetExt() == null)
            {
                string str3 = getShaName(str + ".unity3d");
                str2 = DataServerAddress + str3;
            }
            else
            {
                str2 = DataServerAddress + str;
            }
            if (assetData.GetExt() == null)
            {
                string str4 = string.Empty;
                if (str.Contains("%"))
                {
                    char[] separator = new char[] { '%' };
                    char[] chArray2 = new char[] { '%' };
                    str4 = str.Split(separator)[0] + str.Split(chArray2)[2];
                }
                else
                {
                    str4 = str;
                }
                return (DataServerAddress + getShaName(str4 + ".unity3d"));
            }
            return (DataServerAddress + str);
        }
    }
}
