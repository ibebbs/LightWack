using Bebbs.LightWack.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bebbs.LightWack.ViewModels
{
    public class LedGroupViewModel : PropertyChangedBase
    {
        public static double LedGroupLength = 10.0;
        public static double LedGroupBredth = 1.0;

        private static readonly int Rotations = Enum.GetNames(typeof(Rotation)).Length - 1;
        private readonly IGlobalEventAggregator _eventAggregator;
        private readonly DelegateCommand _rotateCommand;
        private Rotation _rotation;
        private ObservablePoint _location;
        private bool _enabled;
        private bool _lit;

        public LedGroupViewModel(IFactory factory, IGlobalEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _location = new ObservablePoint();
            _rotateCommand = new DelegateCommand(ExecuteRotate, CanExecuteRotate);

            Leds = Enumerable.Range(0, 3).Select(index => factory.Construct<LedViewModel>()).ToArray();
        }

        private void LitChanged()
        {
            _eventAggregator.Publish(new LedStateChanged(0, Group, Lit, Color.White));
        }

        private void ExecuteRotate(object parameter)
        {
            switch (Rotation)
            {
                case System.Windows.Media.Imaging.Rotation.Rotate0:
                    Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;
                    break;
                case System.Windows.Media.Imaging.Rotation.Rotate90:
                    Rotation = System.Windows.Media.Imaging.Rotation.Rotate180;
                    break;
                case System.Windows.Media.Imaging.Rotation.Rotate180:
                    Rotation = System.Windows.Media.Imaging.Rotation.Rotate270;
                    break;
                default:
                    Rotation = System.Windows.Media.Imaging.Rotation.Rotate0;
                    break;
            }
        }

        private bool CanExecuteRotate(object parameter)
        {
            return Enabled;
        }

        public void SetIdentity(int group)
        {
            Group = group;
            Leds.ForEach((index, item) => item.SetIdentity(group, index));
        }

        public int Group { get; private set; }

        public IEnumerable<LedViewModel> Leds { get; private set; }

        public ICommand RotateCommand
        {
            get { return _rotateCommand; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;

                    NotifyOfPropertyChange(() => Enabled);
                }
            }
        }

        public Rotation Rotation 
        {
            get { return _rotation; }
            set
            {
                if (value != _rotation)
                {
                    _rotation = value;

                    NotifyOfPropertyChange(() => Rotation);
                }
            }
        }

        public ObservablePoint Location
        {
            get { return _location; }
        }

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
