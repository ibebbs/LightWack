using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack.Services
{
    public interface ILightPackLed
    {
        int Index { get; }
        bool IsEnabled { get; }
        Point Position { get; }
        Size Size { get; }
    }

    internal class LightPackLed : ILightPackLed
    {
        public LightPackLed(int index, bool isEnabled, Point position, Size size)
        {
            Index = index;
            IsEnabled = isEnabled;
            Position = position;
            Size = size;
        }

        public int Index { get; private set; }

        public bool IsEnabled { get; private set; }

        public Point Position { get; private set; }

        public Size Size { get; private set; }
    }

    public interface ILightPackProfile
    {
        IEnumerable<ILightPackLed> Leds { get; }
    }

    internal class LightPackProfile : ILightPackProfile
    {
        public LightPackProfile(IEnumerable<ILightPackLed> leds)
        {
            Leds = (leds ?? Enumerable.Empty<ILightPackLed>()).ToArray();
        }

        public IEnumerable<ILightPackLed> Leds { get; private set; }
    }
}
