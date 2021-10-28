using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP
{

    class Animal
    {
        internal double RunSpeed;

        internal void SetRunSpeed(double speed)
        {
            this.RunSpeed = speed;
        }

        internal double GetRunTime(double distance)
        {
            return distance / this.RunSpeed;
        }

    }
}
