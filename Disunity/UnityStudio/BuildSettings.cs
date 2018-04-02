namespace Unity_Studio
{
    using System;

    public class BuildSettings
    {
        public string m_Version;

        public BuildSettings(AssetPreloadData preloadData)
        {
            AssetsFile sourceFile = preloadData.sourceFile;
            EndianStream stream = preloadData.sourceFile.a_Stream;
            stream.Position = preloadData.Offset;
            int num = stream.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                string str = stream.ReadAlignedString(stream.ReadInt32());
            }
            if (sourceFile.version[0] == 5)
            {
                int num3 = stream.ReadInt32();
                for (int j = 0; j < num3; j++)
                {
                    string str2 = stream.ReadAlignedString(stream.ReadInt32());
                }
            }
            stream.Position += 4L;
            if (sourceFile.fileGen >= 8)
            {
                stream.Position += 4L;
            }
            if (sourceFile.fileGen >= 9)
            {
                stream.Position += 4L;
            }
            if ((sourceFile.version[0] == 5) || ((sourceFile.version[0] == 4) && ((sourceFile.version[1] >= 3) || ((sourceFile.version[1] == 2) && (sourceFile.buildType[0] != "a")))))
            {
                stream.Position += 4L;
            }
            this.m_Version = stream.ReadAlignedString(stream.ReadInt32());
        }
    }
}

