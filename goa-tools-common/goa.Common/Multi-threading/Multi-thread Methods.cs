using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    public static class MultiThreadMethods
    {
        public static bool WaitForConditionWithTimeOut(ref bool condition, int interval, int timeout)
        {
            bool conditionMet = true;
            int totalTime = 0;
            while (condition == false)
            {
                totalTime += interval;
                Thread.Sleep(interval);
                if (totalTime >= timeout)
                {
                    conditionMet = false;
                    break;
                }
            }

            return conditionMet;
        }
        public static bool WaitForFormToShowWithTimeOut(Form form, int interval, int timeout)
        {
            bool conditionMet = true;
            int totalTime = 0;
            while (form.Visible == false)
            {
                totalTime += interval;
                Thread.Sleep(interval);
                if (totalTime >= timeout)
                {
                    conditionMet = false;
                    break;
                }
            }
            return conditionMet;
        }
    }
}
