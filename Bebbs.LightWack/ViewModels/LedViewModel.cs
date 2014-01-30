using Bebbs.LightWack.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bebbs.LightWack.ViewModels
{
    public class LedViewModel : PropertyChangedBase
    {
        private readonly IGlobalEventAggregator _eventAggregator;
        private bool _lit;

        public LedViewModel(IGlobalEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        private void LitChanged()
        {
            _eventAggregator.Publish(new LedStateChanged(Group, Index, Lit, Color.White));
        }

        public void SetIdentity(int group, int index)
        {
            Group = group;
            Index = index;
        }

        public int Group { get; private set; }
        public int Index { get; private set; }

        public bool Lit
        {
            get { return _lit; }
            set
            {
                if (value != _lit)
                {
                    _lit = value;

                    NotifyOfPropertyChange(() => Lit);

                    LitChanged();
                }
            }
        }
    }
}
