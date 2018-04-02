using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unreal.FGO.Core
{
    public static class SvtExt
    {
        public static string NarrowFigureAssetName(this mstSvt svt)
        {
            return "NarrowFigure/" + svt.id;
        }

        public static string CharaFigureAssetName(this mstSvt svt, int index = 0)
        {
            return "CharaFigure/" + svt.id + index;
        }

        public static mstSvt GetById(this List<mstSvt> list, string svtId)
        {
            return list.Where(s => s.id == svtId || s.name.Contains(svtId)).FirstOrDefault();
        }
    }
}
