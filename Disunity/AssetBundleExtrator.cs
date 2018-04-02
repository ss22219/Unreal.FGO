using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unity_Studio;
using SiliconStudio.TextureConverter.PvrttWrapper;
using System.Drawing.Imaging;

namespace Disunity
{
    public static class AssetBundleExtrator
    {
        [DllImport("AssetsToolsWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void extratAssetBundleFile(string fileName, string descFileName);

        public static void ExtratAssetsFile(string fileName, ushort type = 28)
        {
            var dir = Path.GetDirectoryName(fileName);
            var assetsFileName = Path.GetFileNameWithoutExtension(fileName);
            var assetsFile = new AssetsFile(fileName);
            assetsFile.LoadExportableAssets();
            foreach (var asset in assetsFile.exportableAssets)
            {
                if (!Directory.Exists(Path.Combine(dir, assetsFileName)))
                    Directory.CreateDirectory(Path.Combine(dir, assetsFileName));

                try
                {
                    if (asset.Type2 == type)
                        asset.Exportable.Export(Path.Combine(dir, assetsFileName, asset.ExportName));
                    if (asset.extension == ".pvr")
                    {
                        PvrToPng(Path.Combine(dir, assetsFileName, asset.ExportName));
                        File.Delete(Path.Combine(dir, assetsFileName, asset.ExportName));
                    }
                }
                catch (Exception)
                {
                }
            }

            assetsFile.exportableAssets.Where(a => a.Type2 == 28 && assetsFile.exportableAssets.Any(b => b.Name == a.Name + "a")).ToList().ForEach(asset =>
            {
                if (asset.extension == ".pvr")
                {
                    try
                    {
                        TransToAlphaImage(Path.Combine(dir, assetsFileName, asset.Name + ".png"));
                        File.Delete(Path.Combine(dir, assetsFileName, asset.Name + "a.png"));
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        public unsafe static void TransToAlphaImage(string file)
        {
            using (Image image = Image.FromFile(file))
            {
                var file2 = Path.GetExtension(file);
                if (string.IsNullOrEmpty(file2))
                    file2 = file + "a";
                else
                    file2 = file.Substring(0, file.Length - file2.Length) + "a" + file2;

                using (Image image2 = Image.FromFile(file2))
                {

                    using (Bitmap bitmap = new Bitmap(image))
                    {
                        using (Bitmap bitmap2 = new Bitmap(image2))
                        {
                            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                            var bitmapData2 = bitmap2.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            var src = (uint*)bitmapData2.Scan0;
                            var desc = (uint*)bitmapData.Scan0;
                            int width = image.Width, height = image.Height;
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    if ((*(src + i * width + j)) != 0xffffffff)
                                    {
                                        *(desc + i * width + j) = 0;
                                    }
                                }
                            }
                            image.Dispose();
                            bitmap.Save(file);
                            bitmap.UnlockBits(bitmapData);
                            bitmap2.UnlockBits(bitmapData2);
                            bitmap.Dispose();
                            bitmap2.Dispose();
                            image2.Dispose();
                        }
                    }
                }
            }
        }


        public unsafe static void PvrToPng(string file)
        {
            using (var texture = new PVRTexture(file))
            {
                using (var header = texture.GetHeader())
                {
                    int width = (int)header.GetWidth(), height = (int)header.GetHeight();
                    Utilities.Transcode(texture, PixelType.Standard8PixelType, header.GetChannelType(), header.GetColourSpace(), ECompressorQuality.ePVRTCNormal, true);
                    using (Bitmap bitmap = new Bitmap(width, height))
                    {
                        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        var bpp = 32;
                        var bytes_per_line = (width * bpp);
                        var startPoint = (uint*)bitmapData.Scan0;
                        var texturePoint = (uint*)texture.GetDataPtr();
                        try
                        {
                            for (int line = 0; line < height; line++)
                            {
                                for (int row = 0; row < width; row++)
                                {
                                    int index = line * width + row;
                                    var src = texturePoint + index;
                                    var desc = startPoint + index;
                                    var x = *src;
                                    //abgr to
                                    //argb
                                    (*desc) =
                                        ((x & 0xFF000000)) |
                                        ((x & 0x00FF0000) >> 16) |
                                        ((x & 0x0000FF00)) |
                                        ((x & 0x000000FF) << 16);
                                }
                            }
                            bitmap.UnlockBits(bitmapData);
                            bitmap.Save(Path.ChangeExtension(file, "png"), ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                    }
                }
            }
        }

        public static void Test()
        {
            //TransToAlphaImage(@"C:\Users\Administrator\Desktop\disunity_v0.4.0\bin\1022001.png");
            ExtratAssetsFile(@"C:\Users\Administrator\Desktop\disunity_v0.4.0\bin\3a71869317e5c152a81f102d923f89cd369aabe7_CAB-CharaFigure@1022001.assets");
        }

        public static void ExtratAssetBundleFile(string fileName, string descFileName)
        {
            extratAssetBundleFile(fileName, descFileName);
        }
    }
}
