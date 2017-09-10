using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroscopTest
{
    class Area
    {
        public Point left;
        public Point right;

        public Area()
        {
            left = new Point();
            right = new Point();
        }

        public Area(int x_left, int y_left, int x_right, int y_right)
        {
            left = new Point(x_left, y_left);
            right = new Point(x_right, y_right);
        }

        public override string ToString()
        {
            return string.Format("Left: {0}, {1} - Right: {2}, {3}", left.X, left.Y, right.X, right.Y);
        }
    }
}
