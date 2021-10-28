using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public static partial class FirmStandards
    {
        public static Dictionary<string, BasementFamilyType> BasementFamilytypeID
    = new Dictionary<string, BasementFamilyType>()
    {
                {"BASEMENT-TRANSRADIUS", BasementFamilyType.TransformerRadious }, //公变所服务半径
                {"BASEMENT-TRANSREGION", BasementFamilyType.TransformerServiceRegionTag }, //公变所服务区域标签
                {"BASEMENT-PARKINGSPACE", BasementFamilyType.ParkingSpace }, //停车位
                {"BASEMENT-RAMP", BasementFamilyType.Ramp }, //坡道
                {"BASEMENT-RAMP-STACKED", BasementFamilyType.RampStacked }, //重叠坡道
    };
        public static Guid TransformerRadiousParam_vila = new Guid("1817ccd9-e8c4-4075-a56a-dcc3d7fee750"); //服务半径-别墅
        public static Guid TransformerRadiousParam_highrise = new Guid("bf84337c-3913-4724-8e74-ecd4de55d905"); //服务半径-大高
        public static Guid TransformerRadiousparam_midHighrise = new Guid("b2f013f1-a759-415d-b4c7-39c36db835fb"); //服务半径-小高
        public static Guid TransformerRadiousparam_flat = new Guid("58f5b438-971d-45e8-8483-20e8946c358e"); //服务半径-洋房
        public static Guid TransformerServiceArea_vila = new Guid("85d0518e-60cc-43c5-bfe5-de33cbb979e3"); //服务面积_别墅
        public static Guid TransformerServiceArea_flat = new Guid("4c73614d-c48a-48ed-8cec-0dde111678bd"); //服务面积_洋房
        public static Guid TransformerServiceArea_midHigh = new Guid("c8eb73c4-7989-48ae-afd2-494da0887fbe"); //服务面积_小高
        public static Guid TransformerServiceArea_high = new Guid("97b479c8-7889-44b0-bf93-cc6750d24408"); //服务面积_大高
    }
    public enum BasementFamilyType
    {
        ParkingSpace,
        TransformerRadious,
        TransformerServiceRegionTag,
        Ramp,
        RampStacked,
    }
}
