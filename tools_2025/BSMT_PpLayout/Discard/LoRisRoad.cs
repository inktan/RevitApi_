//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BSMT_PpLayout.Discard
//{
//    class LoRisRoad
//    {
//    }


//    internal enum LoRisRoad : int
//    {
//        None,
//        L,
//        R,
//        LR
//    }

//    internal LoRisRoad LoRisRoad { get; set; }

//    /// <summary>
//    /// 判断与线段两端点相连的边界是否为直角边属性
//    /// </summary>
//    private void JudgeLoRisRoad(IEnumerable<BoundSeg> boundSegs)
//    {
//        Vector2d p0 = this.Segment2d.P0;
//        Vector2d p1 = this.Segment2d.P1;
//        if (p0.x > p1.x)
//        {
//            Vector2d temp = p0;
//            p0 = p1;
//            p1 = temp;
//        }
//        //else if (p0.x.EqualPrecision(p1.x))
//        //{
//        //    if (p0.y < p1.y)
//        //    {
//        //        Vector2d temp = p0;
//        //        p0 = p1;
//        //        p1 = temp;
//        //    }
//        //}

//        Vector2d center = Segment2d.Center;
//        Vector2d directionL = (p0 - center).Normalized;
//        double distance = GlobalData.Wd_pri / 2 + 0.1;
//        Segment2d sgeL = new Segment2d(p0 + directionL * distance, center);// 5000mm的长度，超过道路宽度的一半
//        Segment2d sgeR = new Segment2d(p1 + (directionL * distance * -1), center);

//        IEnumerable<Segment2d> segment2ds = boundSegs.Select(p => p.Segment2d);
//        bool boolL = false;
//        foreach (var item in segment2ds)
//        {
//            IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(sgeL, item);
//            intrSegment2Segment2.Compute();
//            if (intrSegment2Segment2.Quantity == 1)
//            {
//                boolL = true;
//                break;
//            }
//        }
//        bool boolR = false;
//        foreach (var item in segment2ds)
//        {
//            IntrSegment2Segment2 intrSegment2Segment2 = new IntrSegment2Segment2(sgeR, item);
//            intrSegment2Segment2.Compute();
//            if (intrSegment2Segment2.Quantity == 1)
//            {
//                boolR = true;
//                break;
//            }
//        }

//        if (boolL && boolR)
//            this.LoRisRoad = LoRisRoad.LR;
//        else if (!boolL && boolR)
//            this.LoRisRoad = LoRisRoad.R;
//        else if (boolL && !boolR)
//            this.LoRisRoad = LoRisRoad.L;

//    //}
//}
