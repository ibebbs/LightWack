using System;

namespace Bebbs.LightWack.Messages
{
    public class LedMessage
    {
        public LedMessage(int group, int index)
        {
            Group = group;
            Index = index;
        }

        public byte Led
        {
            get { return Convert.ToByte((Group * 3) + Index); }
        }

        public int Group { get; private set; }

        public int Index { get; private set; }
    }
}
