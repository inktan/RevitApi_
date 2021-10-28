using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public static partial class FirmStandards
    {
        public static Guid FamilyTypeIDForAPI = new Guid("679a258b-764c-481b-bd7d-0f543c298849");

        public static string InvalidFileNameCharacters = @"\<>|:*?/";

        public static Dictionary<string, OtherFamilyType> OtherFamilytypeID
         = new Dictionary<string, OtherFamilyType>()
        {
                {"OTHER-CUTTER", OtherFamilyType.Cutter }, //剪切形体族
                {"SPACE-MARK",  OtherFamilyType.SpaceMark}, //使用面积标记
                {"VOID_FACEBASED", OtherFamilyType.VoidFaceBased }, //基于面空心
                {"VOID_LINEBASED", OtherFamilyType.VoidLineBased }, //基于线空心
                {"COOR_3D", OtherFamilyType.Coord3D}, //3D坐标系
                {"COOR_2D", OtherFamilyType.Coord2D }, //2D坐标系
         };
    }
    public enum OtherFamilyType
    {
        Cutter,
        SpaceMark,
        VoidFaceBased,
        VoidLineBased,
        Coord3D,
        Coord2D,
    }
}
