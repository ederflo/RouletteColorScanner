using System;
using System.Collections.Generic;
using System.Text;

namespace RouletteColorScanner
{
    public class ScreenLocation
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ScreenLocation() { }

        public ScreenLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
