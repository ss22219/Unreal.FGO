namespace Unity_Studio
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class AssetPreloadData
    {
        public int exportSize;
        public string extension;
        public string InfoText;
        public long m_PathID;
        public int Offset;
        public int Size;
        public AssetsFile sourceFile;
        public int Type1;
        public ushort Type2;
        public string TypeString;
        public string uniqueID;

        public Unity_Studio.Exportable Exportable { get; set; }

        public string ExportName
        {
            get
            {
                return (this.Name + this.extension);
            }
        }

        public string Name { get; set; }
    }
}

