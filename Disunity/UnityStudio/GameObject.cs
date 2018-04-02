namespace Unity_Studio
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity_Studio;
    public class GameObject
    {
        public bool m_IsActive;
        public int m_Layer;
        public PPtr m_MeshFilter;
        public string m_Name;
        public PPtr m_Renderer;
        public PPtr m_SkinnedMeshRenderer;
        public ushort m_Tag;
        public PPtr m_Transform;
        public string uniqueID = "0";

        public GameObject(AssetPreloadData preloadData)
        {
            if (preloadData == null)
            {
                AssetsFile sourceFile = preloadData.sourceFile;
                EndianStream stream = preloadData.sourceFile.a_Stream;
                stream.Position = preloadData.Offset;
                this.uniqueID = preloadData.uniqueID;
                if (sourceFile.platform == -2)
                {
                    uint num3 = stream.ReadUInt32();
                    PPtr ptr = sourceFile.ReadPPtr();
                    PPtr ptr2 = sourceFile.ReadPPtr();
                }
                int num = stream.ReadInt32();
                for (int i = 0; i < num; i++)
                {
                    switch (stream.ReadInt32())
                    {
                        case 4:
                            this.m_Transform = sourceFile.ReadPPtr();
                            break;

                        case 0x17:
                            this.m_Renderer = sourceFile.ReadPPtr();
                            break;

                        case 0x21:
                            this.m_MeshFilter = sourceFile.ReadPPtr();
                            break;

                        case 0x89:
                            this.m_SkinnedMeshRenderer = sourceFile.ReadPPtr();
                            break;

                        default:
                        {
                            PPtr ptr3 = sourceFile.ReadPPtr();
                            break;
                        }
                    }
                }
                this.m_Layer = stream.ReadInt32();
                int length = stream.ReadInt32();
                this.m_Name = stream.ReadAlignedString(length);
                if (this.m_Name == "")
                {
                    this.m_Name = "GameObject #" + this.uniqueID;
                }
                this.m_Tag = stream.ReadUInt16();
                this.m_IsActive = stream.ReadBoolean();
                this.Text = this.m_Name;
                this.Name = this.uniqueID;
            }
        }

        public string Name { get; private set; }

        public string Text { get; private set; }
    }
}

