namespace Unity_Studio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class PPtrHelpers
    {
        public static PPtr ReadPPtr(this AssetsFile sourceFile)
        {
            PPtr ptr = new PPtr();
            EndianStream stream = sourceFile.a_Stream;
            int num = stream.ReadInt32();
            if ((num >= 0) && (num < sourceFile.sharedAssetsList.Count))
            {
                ptr.m_FileID = sourceFile.sharedAssetsList[num].Index;
            }
            if (sourceFile.fileGen < 14)
            {
                ptr.m_PathID = stream.ReadInt32();
                return ptr;
            }
            ptr.m_PathID = stream.ReadInt64();
            return ptr;
        }

        public static bool TryGetGameObject(this List<AssetsFile> assetsfileList, PPtr m_elm, out GameObject m_GameObject)
        {
            m_GameObject = null;
            if (((m_elm != null) && (m_elm.m_FileID >= 0)) && (m_elm.m_FileID < assetsfileList.Count))
            {
                AssetsFile file = assetsfileList[m_elm.m_FileID];
                if (file.GameObjectList.TryGetValue(m_elm.m_PathID, out m_GameObject))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetPD(this List<AssetsFile> assetsfileList, PPtr m_elm, out AssetPreloadData result)
        {
            result = null;
            if (((m_elm != null) && (m_elm.m_FileID >= 0)) && (m_elm.m_FileID < assetsfileList.Count))
            {
                AssetsFile file = assetsfileList[m_elm.m_FileID];
                if (file.preloadTable.TryGetValue(m_elm.m_PathID, out result))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

