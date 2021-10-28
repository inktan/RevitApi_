using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSP
{
    class Bird :Animal
    {
        internal double flySpeed;
        
        internal virtual void SetSpeed(double speed)
        {
            this.flySpeed = speed;

        }

        internal  double GetFlyTime(double distance)
        {
            return distance / this.flySpeed;
        }
    }
}
