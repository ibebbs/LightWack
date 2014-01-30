using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack
{
    public static class Prismatic
    {
        public static readonly string ProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Prismatik\Profiles");

        public static readonly double PixelsPerCm = 15;
        public static readonly int Left = 0;
        public static readonly int Right = 2560;
        public static readonly int Top = 0;
        public static readonly int Bottom = 1024;

        public static double PositionToCm(double value)
        {
            return 15 * value;
        }
    }
}
