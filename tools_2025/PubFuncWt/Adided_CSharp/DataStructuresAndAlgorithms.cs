using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class DataStruct_Algorithm<T>
    {
        /// <summary>
        /// 嵌套列表自身组合，穷举所有的元素组合可能性
        /// 把一个嵌套列表进行顺序组合，取出第一个列表的各个元素新建列表，将第二个列表的非空元素依次追加到上一层级的各个列表后
        /// 举例：展开 [[1,2][a,b][A,B]]
        /// [1],[2]==>[1,a],[1,b],[2,a],[b]==>[1,a,A],[1,a,B],[1,b,A],[1,b,B],[2,a,A],[2,a,B]
        /// </summary>
        /// <returns></returns>
        public static Queue<List<T>> ListFactorialCcombination(Queue<List<List<T>>> schemeInfos)
        {
            Queue<List<T>> TsQuene = new Queue<List<T>>();
            Queue<List<T>> _TsQuene = new Queue<List<T>>();
            int time = 0;
            while (schemeInfos.Count != 0)
            {
                List<List<T>> designs = schemeInfos.Dequeue();
                if (designs.Count == 0)
                    continue;
                if (time == 0)
                {
                    foreach (var item in designs)
                    {
                        if (item.Count == 0)
                        {
                            continue;
                        }
                        _TsQuene.Enqueue(item);
                    }
                }
                else
                {
                    List<List<T>> temp = new List<List<T>>(); // 收集不为空的元素，或者，数量不为0的列表
                    foreach (var item in designs)
                    {
                        if (item.Count == 0)
                        {
                            continue;
                        }
                        temp.Add(item);// 符合要求则添加
                    }

                    while (TsQuene.Count != 0)
                    {
                        List<T> ts = TsQuene.Dequeue();

                        if (temp.Count == 0)// 没有需要添加的元素，保留上一层，不做处理
                        {
                            _TsQuene.Enqueue(ts);
                        }
                        else
                        {
                            foreach (var item in temp)
                            {
                                List<T> newTs = new List<T>(ts);// 列表重置
                                newTs.AddRange(item);
                                _TsQuene.Enqueue(newTs);
                            }
                        }
                    }
                }
                TsQuene = new Queue<List<T>>(_TsQuene);
                if (_TsQuene.Count != 0)
                {
                    _TsQuene.Clear();
                    time++;
                }
            }
            return TsQuene;
        }
        public static Queue<List<T>> ListFactorialCcombination(Queue<List<T>> schemeInfos)
        {
            Queue<List<T>> TsQuene = new Queue<List<T>>();
            Queue<List<T>> _TsQuene = new Queue<List<T>>();
            int time = 0;
            while (schemeInfos.Count != 0)
            {
                List<T> designs = schemeInfos.Dequeue();
                if (designs.Count == 0)
                    continue;
                if (time == 0)
                {
                    foreach (var item in designs)
                    {
                        List<T> newList = new List<T>() { item };
                        if (item == null)
                        {
                            continue;
                        }
                        _TsQuene.Enqueue(newList);
                    }
                }
                else
                {
                    List<T> temp = new List<T>(); // 收集不为空的元素，或者，数量不为0的列表
                    foreach (var item in designs)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        temp.Add(item);// 符合要求则添加
                    }

                    while (TsQuene.Count != 0)
                    {
                        List<T> ts = TsQuene.Dequeue();

                        if (temp.Count == 0)// 没有需要添加的元素，保留上一层，不做处理
                        {
                            _TsQuene.Enqueue(ts);
                        }
                        else
                        {

                            foreach (var item in temp)
                            {
                                List<T> newTs = new List<T>(ts);// 列表重置
                                newTs.Add(item);

                                _TsQuene.Enqueue(newTs);
                            }
                        }
                    }
                }
                TsQuene = new Queue<List<T>>(_TsQuene);
                if (_TsQuene.Count != 0)
                {
                    _TsQuene.Clear();
                    time++;
                }
            }
            return TsQuene;
        }
    }
}
