namespace Unity_Studio
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class AssetsFile
    {
        public EndianStream a_Stream;
        private bool baseDefinitions;
        public string[] buildType;
        public List<AssetPreloadData> exportableAssets = new List<AssetPreloadData>();
        public int fileGen;
        public string filePath;
        public Dictionary<long, GameObject> GameObjectList = new Dictionary<long, GameObject>();
        public string m_Version = "2.5.0f5";
        public int platform = 0x6000000;
        public string platformStr = "";
        public Dictionary<long, AssetPreloadData> preloadTable = new Dictionary<long, AssetPreloadData>();
        public List<UnityShared> sharedAssetsList;
        private ClassIDReference UnityClassID;
        public int[] version = new int[4];

        public AssetsFile(string fileName, EndianStream fileStream = null)
        {
            List<UnityShared> list1 = new List<UnityShared> {
                new UnityShared()
            };
            this.sharedAssetsList = list1;
            this.UnityClassID = new ClassIDReference();
            this.baseDefinitions = false;
            if (fileStream == null)
            {
                fileStream = new EndianStream(File.OpenRead(fileName), EndianType.BigEndian);
            }
            this.a_Stream = fileStream;
            this.filePath = fileName;
            int num = this.a_Stream.ReadInt32();
            int num2 = this.a_Stream.ReadInt32();
            this.fileGen = this.a_Stream.ReadInt32();
            int num3 = this.a_Stream.ReadInt32();
            this.sharedAssetsList[0].fileName = Path.GetFileName(fileName);
            switch (this.fileGen)
            {
                case 6:
                    this.a_Stream.Position = num2 - num;
                    this.a_Stream.Position += 1L;
                    break;

                case 7:
                    this.a_Stream.Position = num2 - num;
                    this.a_Stream.Position += 1L;
                    this.m_Version = this.a_Stream.ReadStringToNull();
                    break;

                case 8:
                    this.a_Stream.Position = num2 - num;
                    this.a_Stream.Position += 1L;
                    this.m_Version = this.a_Stream.ReadStringToNull();
                    this.platform = this.a_Stream.ReadInt32();
                    break;

                case 9:
                    this.a_Stream.Position += 4L;
                    this.m_Version = this.a_Stream.ReadStringToNull();
                    this.platform = this.a_Stream.ReadInt32();
                    break;

                case 10:
                case 11:
                case 12:
                case 13:
                    return;

                case 14:
                case 15:
                    this.a_Stream.Position += 4L;
                    this.m_Version = this.a_Stream.ReadStringToNull();
                    this.platform = this.a_Stream.ReadInt32();
                    this.baseDefinitions = this.a_Stream.ReadBoolean();
                    break;

                default:
                    return;
            }
            if ((this.platform > 0xff) || (this.platform < 0))
            {
                byte[] bytes = BitConverter.GetBytes(this.platform);
                Array.Reverse(bytes);
                this.platform = BitConverter.ToInt32(bytes, 0);
                this.a_Stream.endian = EndianType.LittleEndian;
            }
            switch (this.platform)
            {
                case -2:
                    this.platformStr = "Unity Package";
                    break;

                case 4:
                    this.platformStr = "OSX";
                    break;

                case 5:
                    this.platformStr = "PC";
                    break;

                case 6:
                    this.platformStr = "Web";
                    break;

                case 7:
                    this.platformStr = "Web streamed";
                    break;

                case 9:
                    this.platformStr = "iOS";
                    break;

                case 10:
                    this.platformStr = "PS3";
                    break;

                case 11:
                    this.platformStr = "Xbox 360";
                    break;

                case 13:
                    this.platformStr = "Android";
                    break;

                case 0x10:
                    this.platformStr = "Google NaCl";
                    break;

                case 0x15:
                    this.platformStr = "WP8";
                    break;

                case 0x19:
                    this.platformStr = "Linux";
                    break;
            }
            int num4 = this.a_Stream.ReadInt32();
            for (int i = 0; i < num4; i++)
            {
                if (this.fileGen < 14)
                {
                    int num10 = this.a_Stream.ReadInt32();
                    string str2 = this.a_Stream.ReadStringToNull();
                    string str3 = this.a_Stream.ReadStringToNull();
                    this.a_Stream.Position += 20L;
                    int num11 = this.a_Stream.ReadInt32();
                    StringBuilder cb = new StringBuilder();
                    for (int m = 0; m < num11; m++)
                    {
                        this.readBase(cb, 1);
                    }
                }
                else
                {
                    this.readBase5();
                }
            }
            if ((this.fileGen >= 7) && (this.fileGen < 14))
            {
                this.a_Stream.Position += 4L;
            }
            int num5 = this.a_Stream.ReadInt32();
            string format = "D" + num5.ToString().Length.ToString();
            for (int j = 0; j < num5; j++)
            {
                if (this.fileGen >= 14)
                {
                    this.a_Stream.AlignStream(4);
                }
                AssetPreloadData data = new AssetPreloadData();
                if (this.fileGen < 14)
                {
                    data.m_PathID = this.a_Stream.ReadInt32();
                }
                else
                {
                    data.m_PathID = this.a_Stream.ReadInt64();
                }
                data.Offset = this.a_Stream.ReadInt32();
                data.Offset += num3;
                data.Size = this.a_Stream.ReadInt32();
                data.Type1 = this.a_Stream.ReadInt32();
                data.Type2 = this.a_Stream.ReadUInt16();
                this.a_Stream.Position += 2L;
                if ((this.fileGen >= 15) && (this.a_Stream.ReadByte() > 0))
                {
                }
                if (this.UnityClassID.Names[data.Type2] == null)
                {
                    data.TypeString = this.UnityClassID.Names[data.Type2];
                }
                data.uniqueID = j.ToString(format);
                data.exportSize = data.Size;
                data.sourceFile = this;
                this.preloadTable.Add(data.m_PathID, data);
                if ((data.Type2 == 0x8d) && (this.fileGen == 6))
                {
                    long position = this.a_Stream.Position;
                    BuildSettings settings = new BuildSettings(data);
                    this.m_Version = settings.m_Version;
                    this.a_Stream.Position = position;
                }
            }
            string[] separator = new string[] { ".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            this.buildType = this.m_Version.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string[] textArray2 = new string[] { 
                ".", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o",
                "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "\n"
            };
            string[] array = this.m_Version.Split(textArray2, StringSplitOptions.RemoveEmptyEntries);
            this.version = Array.ConvertAll<string, int>(array, new Converter<string, int>(int.Parse));
            if (this.fileGen >= 14)
            {
                int num17 = this.a_Stream.ReadInt32();
                for (int n = 0; n < num17; n++)
                {
                    int num19 = this.a_Stream.ReadInt32();
                    this.a_Stream.AlignStream(4);
                    long num20 = this.a_Stream.ReadInt64();
                }
            }
            int num6 = this.a_Stream.ReadInt32();
            for (int k = 0; k < num6; k++)
            {
                UnityShared item = new UnityShared {
                    aName = this.a_Stream.ReadStringToNull()
                };
                this.a_Stream.Position += 20L;
                item.fileName = this.a_Stream.ReadStringToNull().Replace("/", @"\");
                this.sharedAssetsList.Add(item);
            }
        }

        ~AssetsFile()
        {
            this.a_Stream.Dispose();
        }

        public void LoadExportableAssets()
        {
            foreach (AssetPreloadData data in this.preloadTable.Values)
            {
                switch (data.Type2)
                {
                    case 1:
                    {
                        GameObject obj2 = new GameObject(data);
                        this.GameObjectList.Add(data.m_PathID, obj2);
                        break;
                    }
                    case 0x1c:
                    {
                        Texture2D textured = new Texture2D(data, true);
                        this.exportableAssets.Add(data);
                        data.Exportable = textured;
                        break;
                    }
                    case 0x30:
                    case 0x31:
                    {
                        TextAsset asset = new TextAsset(data, true);
                        this.exportableAssets.Add(data);
                        data.Exportable = asset;
                        break;
                    }
                }
            }
        }

        private void readBase(StringBuilder cb, int level)
        {
            string str = this.a_Stream.ReadStringToNull();
            string str2 = this.a_Stream.ReadStringToNull();
            int num = this.a_Stream.ReadInt32();
            int num2 = this.a_Stream.ReadInt32();
            int num3 = this.a_Stream.ReadInt32();
            int num4 = this.a_Stream.ReadInt32();
            int num5 = this.a_Stream.ReadInt16();
            int num6 = this.a_Stream.ReadInt16();
            int num7 = this.a_Stream.ReadInt32();
            object[] args = new object[] { new string('\t', level), str, str2, num };
            cb.AppendFormat("{0}{1} {2} {3}\r\n", args);
            for (int i = 0; i < num7; i++)
            {
                this.readBase(cb, level + 1);
            }
        }

        private void readBase5()
        {
            if (this.a_Stream.ReadInt32() < 0)
            {
                this.a_Stream.Position += 0x10L;
            }
            this.a_Stream.Position += 0x10L;
            if (this.baseDefinitions)
            {
                string[] strArray = new string[0x3ef];
                strArray[0] = "AABB";
                strArray[5] = "AnimationClip";
                strArray[0x13] = "AnimationCurve";
                strArray[0x31] = "Array";
                strArray[0x37] = "Base";
                strArray[60] = "BitField";
                strArray[0x4c] = "bool";
                strArray[0x51] = "char";
                strArray[0x56] = "ColorRGBA";
                strArray[0x6a] = "data";
                strArray[0x8a] = "FastPropertyName";
                strArray[0x9b] = "first";
                strArray[0xa1] = "float";
                strArray[0xa7] = "Font";
                strArray[0xac] = "GameObject";
                strArray[0xb7] = "Generic Mono";
                strArray[0xd0] = "GUID";
                strArray[0xde] = "int";
                strArray[0xf1] = "map";
                strArray[0xf5] = "Matrix4x4f";
                strArray[0x106] = "NavMeshSettings";
                strArray[0x107] = "MonoBehaviour";
                strArray[0x115] = "MonoScript";
                strArray[0x12b] = "m_Curve";
                strArray[0x15d] = "m_Enabled";
                strArray[0x176] = "m_GameObject";
                strArray[0x1ab] = "m_Name";
                strArray[490] = "m_Script";
                strArray[0x207] = "m_Type";
                strArray[0x20e] = "m_Version";
                strArray[0x21f] = "pair";
                strArray[0x224] = "PPtr<Component>";
                strArray[0x234] = "PPtr<GameObject>";
                strArray[0x245] = "PPtr<Material>";
                strArray[0x268] = "PPtr<MonoScript>";
                strArray[0x279] = "PPtr<Object>";
                strArray[0x2b0] = "PPtr<Texture>";
                strArray[0x2be] = "PPtr<Texture2D>";
                strArray[0x2ce] = "PPtr<Transform>";
                strArray[0x2e5] = "Quaternionf";
                strArray[0x2f1] = "Rectf";
                strArray[0x30a] = "second";
                strArray[0x31b] = "size";
                strArray[800] = "SInt16";
                strArray[0x32e] = "int64";
                strArray[840] = "string";
                strArray[0x36a] = "Texture2D";
                strArray[0x374] = "Transform";
                strArray[0x37e] = "TypelessData";
                strArray[0x38b] = "UInt16";
                strArray[0x3a0] = "UInt8";
                strArray[0x3a6] = "unsigned int";
                strArray[0x3d5] = "vector";
                strArray[0x3dc] = "Vector2f";
                strArray[0x3e5] = "Vector3f";
                strArray[0x3ee] = "Vector4f";
                int num2 = this.a_Stream.ReadInt32();
                int count = this.a_Stream.ReadInt32();
                this.a_Stream.Position += num2 * 0x18;
                string str = Encoding.UTF8.GetString(this.a_Stream.ReadBytes(count));
                string str2 = "";
                StringBuilder builder = new StringBuilder();
                this.a_Stream.Position -= (num2 * 0x18) + count;
                for (int i = 0; i < num2; i++)
                {
                    string str3;
                    string str4;
                    ushort num5 = this.a_Stream.ReadUInt16();
                    byte num6 = this.a_Stream.ReadByte();
                    bool flag3 = this.a_Stream.ReadBoolean();
                    ushort startIndex = this.a_Stream.ReadUInt16();
                    if (this.a_Stream.ReadUInt16() == 0)
                    {
                        str3 = str.Substring(startIndex, str.IndexOf('\0', startIndex) - startIndex);
                    }
                    else
                    {
                        str3 = (strArray[startIndex] != null) ? strArray[startIndex] : startIndex.ToString();
                    }
                    ushort num9 = this.a_Stream.ReadUInt16();
                    if (this.a_Stream.ReadUInt16() == 0)
                    {
                        str4 = str.Substring(num9, str.IndexOf('\0', num9) - num9);
                    }
                    else
                    {
                        str4 = (strArray[num9] != null) ? strArray[num9] : num9.ToString();
                    }
                    int num10 = this.a_Stream.ReadInt32();
                    int num11 = this.a_Stream.ReadInt32();
                    int num12 = this.a_Stream.ReadInt32();
                    if (num11 == 0)
                    {
                        str2 = str3 + " " + str4;
                    }
                    else
                    {
                        object[] args = new object[] { new string('\t', num6), str3, str4, num10 };
                        builder.AppendFormat("{0}{1} {2} {3}\r\n", args);
                    }
                }
                this.a_Stream.Position += count;
            }
        }

        public class UnityShared
        {
            public string aName = "";
            public string fileName = "";
            public int Index = -1;
        }
    }
}

