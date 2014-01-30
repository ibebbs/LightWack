using System.Drawing;

namespace Bebbs.LightWack.Messages
{
    public class LedStateChanged : LedMessage
    {
        public LedStateChanged(int group, int index, bool lit, Color color) : base(group, index)
        {
            Lit = lit;
            Color = color;
        }

        public bool Lit { get; private set; }

        public Color Color { get; private set; }
    }
}
