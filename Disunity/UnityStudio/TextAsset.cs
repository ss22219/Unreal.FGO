namespace Unity_Studio
{
    using SevenZip.Compression.LZMA;
    using System;
    using System.IO;

    internal class TextAsset : Exportable
    {
        public string m_Name;
        public string m_PathName;
        public byte[] m_Script;

        public TextAsset(AssetPreloadData preloadData, bool readSwitch)
        {
            AssetsFile sourceFile = preloadData.sourceFile;
            EndianStream stream = preloadData.sourceFile.a_Stream;
            stream.Position = preloadData.Offset;
            preloadData.extension = ".txt";
            if (sourceFile.platform == -2)
            {
                uint num2 = stream.ReadUInt32();
                PPtr ptr = sourceFile.ReadPPtr();
                PPtr ptr2 = sourceFile.ReadPPtr();
            }
            this.m_Name = stream.ReadAlignedString(stream.ReadInt32());
            if (this.m_Name != "")
            {
                preloadData.Name = this.m_Name;
            }
            else
            {
                preloadData.Name = preloadData.TypeString + " #" + preloadData.uniqueID;
            }
            int count = stream.ReadInt32();
            if (readSwitch)
            {
                this.m_Script = new byte[count];
                stream.Read(this.m_Script, 0, count);
                if (this.m_Script[0] == 0x5d)
                {
                    this.m_Script = SevenZipHelper.Decompress(this.m_Script);
                }
                if ((this.m_Script[0] == 60) || ((((this.m_Script[0] == 0xef) && (this.m_Script[1] == 0xbb)) && (this.m_Script[2] == 0xbf)) && (this.m_Script[3] == 60)))
                {
                    preloadData.extension = ".xml";
                }
            }
            else
            {
                if (stream.ReadByte() == 0x5d)
                {
                    stream.Position += 4L;
                    preloadData.exportSize = stream.ReadInt32();
                    stream.Position -= 8L;
                }
                else
                {
                    preloadData.exportSize = count;
                }
                stream.Position += count - 1;
            }
            stream.AlignStream(4);
            this.m_PathName = stream.ReadAlignedString(stream.ReadInt32());
        }

        public void Export(string exportFilename)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(exportFilename, FileMode.Create)))
            {
                writer.Write(this.m_Script);
                writer.Close();
            }
        }
    }
}

