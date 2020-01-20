using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding.Dstar
{
    public class Robot
    {
        public Robot(int StartX, int Starty)
        {
            X = StartX;
            Y = Starty;
            Theda = 0;
            Accel = 0;
            Velocity = 0;
        }
        public int X, Y,Accel,Velocity,Theda;

    }
}
