using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack
{
    public class ObservablePoint : PropertyChangedBase
    {
        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;

                    NotifyOfPropertyChange(() => X);
                }
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;

                    NotifyOfPropertyChange(() => Y);
                }
            }
        }
    }
}
