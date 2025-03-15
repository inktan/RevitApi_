using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    /// <summary>
    /// 浮点数区间类 double 默认放大 1e-6
    /// </summary>
    class Interval
    {
        private double _start = 0.0, _end = 0.0;
        public double Start
        {
            get { return Math.Min(this._start, this._end); }
          
        }
        public double End
        {
            get { return Math.Max(this._start,this._end); }
          
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Interval(double start, double end)
        {
            this._start = start;
            this._end = end;
        }
        /// <summary>
        /// 在不在区间内，包含端点
        /// </summary>
        public bool Inside(double tarValue)
        {
            return (Start <= tarValue && tarValue <= End);
        }
        /// <summary>
        /// 在不在区间内，不包含端点
        /// </summary>
        public bool InsideWithOutEndpoint(double tarValue)
        {
            return (Start < tarValue && tarValue < End);
        }
        /// <summary>
        /// 区间求并运算，并返回新的实例；若不可合并，则返回null
        /// </summary>
        /// <param name="otherInterval"></param>
        /// <returns></returns>
        public Interval Merge(Interval otherInterval)
        {
            if (this.Start > otherInterval.End || this.End < otherInterval.Start)
            {
                return null;
            }
            else
            {
                double start = Math.Min(this.Start, otherInterval.Start);
                double end = Math.Max(this.End, otherInterval.End);
                return new Interval(start, end);
            }
        }
    }
}
