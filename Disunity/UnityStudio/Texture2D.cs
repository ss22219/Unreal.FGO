namespace Unity_Studio
{
    using SiliconStudio.TextureConverter.PvrttWrapper;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class Texture2D : Exportable
    {
        public int dwABitMask;
        public int dwBBitMask;
        public int dwCaps = 0x1000;
        public int dwCaps2 = 0;
        public int dwFlags = 0x1007;
        public int dwFlags2;
        public int dwFourCC = 0;
        public int dwGBitMask;
        public int dwMipMapCount = 1;
        public int dwPitchOrLinearSize = 0;
        public int dwRBitMask;
        public int dwRGBBitCount;
        public int dwSize = 0x20;
        public byte[] image_data;
        public int image_data_size;
        public int m_Aniso;
        public int m_ColorSpace;
        public int m_CompleteImageSize;
        public int m_FilterMode;
        public int m_Height;
        public int m_ImageCount;
        public bool m_IsReadable;
        public int m_LightmapFormat;
        public float m_MipBias;
        public bool m_MipMap = false;
        public string m_Name;
        public bool m_ReadAllowed;
        public int m_TextureDimension;
        public int m_TextureFormat;
        public int m_Width;
        public int m_WrapMode;
        public int pvrChannelType = 0;
        public int pvrColourSpace = 0;
        public int pvrDepth = 1;
        public int pvrFlags = 0;
        public int pvrMetaDataSize = 0;
        public int pvrNumFaces = 1;
        public int pvrNumSurfaces = 1;
        public long pvrPixelFormat;
        public int pvrVersion = 0x3525650;

        public Texture2D(AssetPreloadData preloadData, bool readSwitch)
        {
            AssetsFile sourceFile = preloadData.sourceFile;
            EndianStream stream = preloadData.sourceFile.a_Stream;
            stream.Position = preloadData.Offset;
            if (sourceFile.platform == -2)
            {
                uint num = stream.ReadUInt32();
                PPtr ptr = sourceFile.ReadPPtr();
                PPtr ptr2 = sourceFile.ReadPPtr();
            }
            this.m_Name = stream.ReadAlignedString(stream.ReadInt32());
            this.m_Width = stream.ReadInt32();
            this.m_Height = stream.ReadInt32();
            this.m_CompleteImageSize = stream.ReadInt32();
            this.m_TextureFormat = stream.ReadInt32();
            this.getExtension(preloadData, this.m_TextureFormat);
            if (this.m_Name != "")
            {
                preloadData.Name = this.m_Name;
            }
            else
            {
                preloadData.Name = preloadData.TypeString + " #" + preloadData.uniqueID;
            }
            if ((sourceFile.version[0] < 5) || ((sourceFile.version[0] == 5) && (sourceFile.version[1] < 2)))
            {
                this.m_MipMap = stream.ReadBoolean();
            }
            else
            {
                this.dwFlags += 0x20000;
                this.dwMipMapCount = stream.ReadInt32();
                this.dwCaps += 0x400008;
            }
            this.m_IsReadable = stream.ReadBoolean();
            this.m_ReadAllowed = stream.ReadBoolean();
            stream.AlignStream(4);
            this.m_ImageCount = stream.ReadInt32();
            this.m_TextureDimension = stream.ReadInt32();
            this.m_FilterMode = stream.ReadInt32();
            this.m_Aniso = stream.ReadInt32();
            this.m_MipBias = stream.ReadSingle();
            this.m_WrapMode = stream.ReadInt32();
            if (sourceFile.version[0] >= 3)
            {
                this.m_LightmapFormat = stream.ReadInt32();
                if ((sourceFile.version[0] >= 4) || (sourceFile.version[1] >= 5))
                {
                    this.m_ColorSpace = stream.ReadInt32();
                }
            }
            this.image_data_size = stream.ReadInt32();
            if (this.m_MipMap)
            {
                this.dwFlags += 0x20000;
                this.dwMipMapCount = Convert.ToInt32((double)(Math.Log((double)Math.Max(this.m_Width, this.m_Height)) / Math.Log(2.0)));
                this.dwCaps += 0x400008;
            }
            if (!readSwitch)
            {
                string[] textArray1 = new string[] { "Width: ", this.m_Width.ToString(), "\nHeight: ", this.m_Height.ToString(), "\nFormat: " };
                preloadData.InfoText = string.Concat(textArray1);
                preloadData.exportSize = this.image_data_size;
                switch (this.m_TextureFormat)
                {
                    case 1:
                        preloadData.InfoText = preloadData.InfoText + "Alpha8";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 2:
                        preloadData.InfoText = preloadData.InfoText + "ARGB 4.4.4.4";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 3:
                        preloadData.InfoText = preloadData.InfoText + "BGR 8.8.8";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 4:
                        preloadData.InfoText = preloadData.InfoText + "GRAB 8.8.8.8";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 5:
                        preloadData.InfoText = preloadData.InfoText + "BGRA 8.8.8.8";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 7:
                        preloadData.InfoText = preloadData.InfoText + "RGB 5.6.5";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 10:
                        preloadData.InfoText = preloadData.InfoText + "DXT1";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 12:
                        preloadData.InfoText = preloadData.InfoText + "DXT5";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 13:
                        preloadData.InfoText = preloadData.InfoText + "RGBA 4.4.4.4";
                        preloadData.extension = ".dds";
                        preloadData.exportSize += 0x80;
                        goto Label_0E7B;

                    case 0x1c:
                        preloadData.InfoText = preloadData.InfoText + "DXT1 Crunched";
                        preloadData.extension = ".crn";
                        goto Label_0E7B;

                    case 0x1d:
                        preloadData.InfoText = preloadData.InfoText + "DXT5 Crunched";
                        preloadData.extension = ".crn";
                        goto Label_0E7B;

                    case 30:
                        preloadData.InfoText = preloadData.InfoText + "PVRTC_RGB2";
                        preloadData.extension = ".pvr";
                        preloadData.exportSize += 0x34;
                        goto Label_0E7B;

                    case 0x1f:
                        preloadData.InfoText = preloadData.InfoText + "PVRTC_RGBA2";
                        preloadData.extension = ".pvr";
                        preloadData.exportSize += 0x34;
                        goto Label_0E7B;

                    case 0x20:
                        preloadData.InfoText = preloadData.InfoText + "PVRTC_RGB4";
                        preloadData.extension = ".pvr";
                        preloadData.exportSize += 0x34;
                        goto Label_0E7B;

                    case 0x21:
                        preloadData.InfoText = preloadData.InfoText + "PVRTC_RGBA4";
                        preloadData.extension = ".pvr";
                        preloadData.exportSize += 0x34;
                        goto Label_0E7B;

                    case 0x22:
                        preloadData.InfoText = preloadData.InfoText + "ETC_RGB4";
                        preloadData.extension = ".pvr";
                        preloadData.exportSize += 0x34;
                        goto Label_0E7B;
                }
                preloadData.InfoText = preloadData.InfoText + "unknown";
                preloadData.extension = ".tex";
            }
            else
            {
                this.image_data = new byte[this.image_data_size];
                stream.Read(this.image_data, 0, this.image_data_size);
                switch (this.m_TextureFormat)
                {
                    case 1:
                        this.dwFlags2 = 2;
                        this.dwRGBBitCount = 8;
                        this.dwRBitMask = 0;
                        this.dwGBitMask = 0;
                        this.dwBBitMask = 0;
                        this.dwABitMask = 0xff;
                        return;

                    case 2:
                        if (sourceFile.platform != 11)
                        {
                            if (sourceFile.platform == 13)
                            {
                                for (int j = 0; j < (this.image_data_size / 2); j++)
                                {
                                    byte[] buffer1 = new byte[] { this.image_data[j * 2], this.image_data[(j * 2) + 1], this.image_data[j * 2], this.image_data[(j * 2) + 1] };
                                    byte[] bytes = BitConverter.GetBytes((int)(BitConverter.ToInt32(buffer1, 0) >> 4));
                                    this.image_data[j * 2] = bytes[0];
                                    this.image_data[(j * 2) + 1] = bytes[1];
                                }
                            }
                            break;
                        }
                        for (int i = 0; i < (this.image_data_size / 2); i++)
                        {
                            byte num4 = this.image_data[i * 2];
                            this.image_data[i * 2] = this.image_data[(i * 2) + 1];
                            this.image_data[(i * 2) + 1] = num4;
                        }
                        break;

                    case 3:
                        for (int k = 0; k < (this.image_data_size / 3); k++)
                        {
                            byte num7 = this.image_data[k * 3];
                            this.image_data[k * 3] = this.image_data[(k * 3) + 2];
                            this.image_data[(k * 3) + 2] = num7;
                        }
                        this.dwFlags2 = 0x40;
                        this.dwRGBBitCount = 0x18;
                        this.dwRBitMask = 0xff0000;
                        this.dwGBitMask = 0xff00;
                        this.dwBBitMask = 0xff;
                        this.dwABitMask = 0;
                        return;

                    case 4:
                        for (int m = 0; m < (this.image_data_size / 4); m++)
                        {
                            byte num9 = this.image_data[m * 4];
                            this.image_data[m * 4] = this.image_data[(m * 4) + 2];
                            this.image_data[(m * 4) + 2] = num9;
                        }
                        this.dwFlags2 = 0x41;
                        this.dwRGBBitCount = 0x20;
                        this.dwRBitMask = 0xff0000;
                        this.dwGBitMask = 0xff00;
                        this.dwBBitMask = 0xff;
                        this.dwABitMask = -16777216;
                        return;

                    case 5:
                        for (int n = 0; n < (this.image_data_size / 4); n++)
                        {
                            byte num11 = this.image_data[n * 4];
                            byte num12 = this.image_data[(n * 4) + 1];
                            this.image_data[n * 4] = this.image_data[(n * 4) + 3];
                            this.image_data[(n * 4) + 1] = this.image_data[(n * 4) + 2];
                            this.image_data[(n * 4) + 2] = num12;
                            this.image_data[(n * 4) + 3] = num11;
                        }
                        this.dwFlags2 = 0x41;
                        this.dwRGBBitCount = 0x20;
                        this.dwRBitMask = 0xff0000;
                        this.dwGBitMask = 0xff00;
                        this.dwBBitMask = 0xff;
                        this.dwABitMask = -16777216;
                        return;

                    case 6:
                    case 8:
                    case 9:
                    case 11:
                    case 0x1c:
                    case 0x1d:
                        return;

                    case 7:
                        if (sourceFile.platform == 11)
                        {
                            for (int num13 = 0; num13 < (this.image_data_size / 2); num13++)
                            {
                                byte num14 = this.image_data[num13 * 2];
                                this.image_data[num13 * 2] = this.image_data[(num13 * 2) + 1];
                                this.image_data[(num13 * 2) + 1] = num14;
                            }
                        }
                        this.dwFlags2 = 0x40;
                        this.dwRGBBitCount = 0x10;
                        this.dwRBitMask = 0xf800;
                        this.dwGBitMask = 0x7e0;
                        this.dwBBitMask = 0x1f;
                        this.dwABitMask = 0;
                        return;

                    case 10:
                        if (sourceFile.platform == 11)
                        {
                            for (int num15 = 0; num15 < (this.image_data_size / 2); num15++)
                            {
                                byte num16 = this.image_data[num15 * 2];
                                this.image_data[num15 * 2] = this.image_data[(num15 * 2) + 1];
                                this.image_data[(num15 * 2) + 1] = num16;
                            }
                        }
                        if (this.m_MipMap)
                        {
                            this.dwPitchOrLinearSize = (this.m_Height * this.m_Width) / 2;
                        }
                        this.dwFlags2 = 4;
                        this.dwFourCC = 0x31545844;
                        this.dwRGBBitCount = 0;
                        this.dwRBitMask = 0;
                        this.dwGBitMask = 0;
                        this.dwBBitMask = 0;
                        this.dwABitMask = 0;
                        return;

                    case 12:
                        if (sourceFile.platform == 11)
                        {
                            for (int num17 = 0; num17 < (this.image_data_size / 2); num17++)
                            {
                                byte num18 = this.image_data[num17 * 2];
                                this.image_data[num17 * 2] = this.image_data[(num17 * 2) + 1];
                                this.image_data[(num17 * 2) + 1] = num18;
                            }
                        }
                        if (this.m_MipMap)
                        {
                            this.dwPitchOrLinearSize = (this.m_Height * this.m_Width) / 2;
                        }
                        this.dwFlags2 = 4;
                        this.dwFourCC = 0x35545844;
                        this.dwRGBBitCount = 0;
                        this.dwRBitMask = 0;
                        this.dwGBitMask = 0;
                        this.dwBBitMask = 0;
                        this.dwABitMask = 0;
                        return;

                    case 13:
                        for (int num19 = 0; num19 < (this.image_data_size / 2); num19++)
                        {
                            byte[] buffer3 = new byte[] { this.image_data[num19 * 2], this.image_data[(num19 * 2) + 1], this.image_data[num19 * 2], this.image_data[(num19 * 2) + 1] };
                            byte[] buffer2 = BitConverter.GetBytes((int)(BitConverter.ToInt32(buffer3, 0) >> 4));
                            this.image_data[num19 * 2] = buffer2[0];
                            this.image_data[(num19 * 2) + 1] = buffer2[1];
                        }
                        this.dwFlags2 = 0x41;
                        this.dwRGBBitCount = 0x10;
                        this.dwRBitMask = 0xf00;
                        this.dwGBitMask = 240;
                        this.dwBBitMask = 15;
                        this.dwABitMask = 0xf000;
                        return;

                    case 30:
                        this.pvrPixelFormat = 0L;
                        return;

                    case 0x1f:
                        this.pvrPixelFormat = 1L;
                        return;

                    case 0x20:
                        this.pvrPixelFormat = 2L;
                        return;

                    case 0x21:
                        this.pvrPixelFormat = 3L;
                        return;

                    case 0x22:
                        this.pvrPixelFormat = 0x16L;
                        return;

                    default:
                        return;
                }
                this.dwFlags2 = 0x41;
                this.dwRGBBitCount = 0x10;
                this.dwRBitMask = 0xf00;
                this.dwGBitMask = 240;
                this.dwBBitMask = 15;
                this.dwABitMask = 0xf000;
                return;
            }
            Label_0E7B:
            switch (this.m_FilterMode)
            {
                case 0:
                    preloadData.InfoText = preloadData.InfoText + "\nFilter Mode: Point ";
                    break;

                case 1:
                    preloadData.InfoText = preloadData.InfoText + "\nFilter Mode: Bilinear ";
                    break;

                case 2:
                    preloadData.InfoText = preloadData.InfoText + "\nFilter Mode: Trilinear ";
                    break;
            }
            AssetPreloadData data = preloadData;
            string[] textArray2 = new string[] { data.InfoText, "\nAnisotropic level: ", this.m_Aniso.ToString(), "\nMip map bias: ", this.m_MipBias.ToString() };
            data.InfoText = string.Concat(textArray2);
            switch (this.m_WrapMode)
            {
                case 0:
                    preloadData.InfoText = preloadData.InfoText + "\nWrap mode: Repeat";
                    break;

                case 1:
                    preloadData.InfoText = preloadData.InfoText + "\nWrap mode: Clamp";
                    break;
            }
        }

        public void Export(string exportFilename)
        {
            switch (this.m_TextureFormat)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 7:
                case 10:
                case 12:
                case 13:
                    using (BinaryWriter writer = new BinaryWriter(File.Open(exportFilename, FileMode.Create)))
                    {
                        writer.Write(0x20534444);
                        writer.Write(0x7c);
                        writer.Write(this.dwFlags);
                        writer.Write(this.m_Height);
                        writer.Write(this.m_Width);
                        writer.Write(this.dwPitchOrLinearSize);
                        writer.Write(0);
                        writer.Write(this.dwMipMapCount);
                        writer.Write(new byte[0x2c]);
                        writer.Write(this.dwSize);
                        writer.Write(this.dwFlags2);
                        writer.Write(this.dwFourCC);
                        writer.Write(this.dwRGBBitCount);
                        writer.Write(this.dwRBitMask);
                        writer.Write(this.dwGBitMask);
                        writer.Write(this.dwBBitMask);
                        writer.Write(this.dwABitMask);
                        writer.Write(this.dwCaps);
                        writer.Write(this.dwCaps2);
                        writer.Write(new byte[12]);
                        writer.Write(this.image_data);
                        writer.Close();
                    }
                    return;

                case 30:
                case 0x1f:
                case 0x20:
                case 0x21:
                case 0x22:
                    {
                        using (PVRTextureHeader headerIn = new PVRTextureHeader((ulong)((uint)this.pvrPixelFormat), this.m_Height, this.m_Width, 1, 1, 1, 1, EPVRTColourSpace.ePVRTCSpacelRGB, EPVRTVariableType.ePVRTVarTypeUnsignedByteNorm, false))
                        {
                            IntPtr destination = Marshal.AllocHGlobal(this.image_data.Length);
                            try
                            {
                                Marshal.Copy(this.image_data, 0, destination, this.image_data.Length);
                                using (PVRTexture sTexture = new PVRTexture(headerIn, destination))
                                {
                                    Utilities.Transcode(sTexture, PixelType.Standard8PixelType, headerIn.GetChannelType(), headerIn.GetColourSpace(), ECompressorQuality.ePVRTCNormal, true);
                                    Utilities.Rotate90(sTexture, EPVRTAxis.ePVRTAxisZ, true);
                                    Utilities.Rotate90(sTexture, EPVRTAxis.ePVRTAxisZ, true);
                                    sTexture.Save(exportFilename);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(destination);
                            }
                            return;
                        }
                    }
            }
            using (BinaryWriter writer2 = new BinaryWriter(File.Open(exportFilename, FileMode.Create)))
            {
                writer2.Write(this.image_data);
                writer2.Close();
            }
        }

        private void getExtension(AssetPreloadData preloadData, int m_TextureFormat)
        {
            switch (m_TextureFormat)
            {
                case 1:
                    preloadData.extension = ".dds";
                    return;

                case 2:
                    preloadData.extension = ".dds";
                    return;

                case 3:
                    preloadData.extension = ".dds";
                    return;

                case 4:
                    preloadData.extension = ".dds";
                    return;

                case 5:
                    preloadData.extension = ".dds";
                    return;

                case 7:
                    preloadData.extension = ".dds";
                    return;

                case 10:
                    preloadData.extension = ".dds";
                    return;

                case 12:
                    preloadData.extension = ".dds";
                    return;

                case 13:
                    preloadData.extension = ".dds";
                    return;

                case 0x1c:
                    preloadData.extension = ".crn";
                    return;

                case 0x1d:
                    preloadData.extension = ".crn";
                    return;

                case 30:
                    preloadData.extension = ".pvr";
                    return;

                case 0x1f:
                    preloadData.extension = ".pvr";
                    return;

                case 0x20:
                    preloadData.extension = ".pvr";
                    return;

                case 0x21:
                    preloadData.extension = ".pvr";
                    return;

                case 0x22:
                    preloadData.extension = ".pvr";
                    return;
            }
            preloadData.extension = ".tex";
        }
    }
}

