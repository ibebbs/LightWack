using Bebbs.LightWack.Behaviors;
using Bebbs.LightWack.Services;
using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Bebbs.LightWack.ViewModels
{
    public class ShellViewModel : Screen, IShell, IDragDropManager
    {
        private readonly ILightPackProfileService _profileService;
        private readonly ObservableCollection<LedGroupViewModel> _ledGroups;
        private readonly DelegateCommand _importCommand;
        private readonly DelegateCommand _exportCommand;

        private double _viewWidth;
        private double _viewHeight;
        private double _monitorWidth;
        private double _monitorHeight;
        private double _viewZoom;

        public ShellViewModel(IFactory factory, ILightPackProfileService profileService, IGlobalEventAggregator eventAggregator)
        {
            _profileService = profileService;

            _ledGroups = new ObservableCollection<LedGroupViewModel>(Enumerable.Range(1, 10).Select(
                group =>
                {
                    LedGroupViewModel ledGroup = factory.Construct<LedGroupViewModel>();
                    ledGroup.SetIdentity(group);
                    return ledGroup;
                }
            ));

            EnabledLedGroupCollection = new CollectionViewSource { Source = _ledGroups }.View as ListCollectionView;
            EnabledLedGroupCollection.Filter = source => source is LedGroupViewModel && ((LedGroupViewModel)source).Enabled;
            EnabledLedGroupCollection.IsLiveFiltering = true;
            EnabledLedGroupCollection.LiveFilteringProperties.Add("Enabled");

            DisabledLedGroupCollection = new CollectionViewSource { Source = _ledGroups }.View as ListCollectionView;
            DisabledLedGroupCollection.Filter = source => source is LedGroupViewModel && !((LedGroupViewModel)source).Enabled;
            DisabledLedGroupCollection.IsLiveFiltering = true;
            DisabledLedGroupCollection.LiveFilteringProperties.Add("Enabled");

            _importCommand = new DelegateCommand(ExecuteImport, obj => true);
            _exportCommand = new DelegateCommand(ExecuteExport, obj => true);

            _viewWidth = 80;
            _viewHeight = 40;
            _viewZoom = 1;
            _monitorWidth = 69.5;
            _monitorHeight = 31.5;
        }

        private void ExecuteImport(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Prismatic.ProfilePath,
                DefaultExt = "ini",
                Filter = "Ini Files (*.ini)|*.ini|All Files (*.*)|*.*",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() ?? false)
            {
                ImportProfile(openFileDialog.FileName);
            }
        }

        private void ExecuteExport(object param)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Prismatic.ProfilePath,
                DefaultExt = "ini",
                Filter = "Ini Files (*.ini)|*.ini|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() ?? false)
            {
                ExportProfile(saveFileDialog.FileName);
            }
        }

        private Point CalculateLocation(System.Drawing.Point source)
        {
            Xaml.CmToPixelConverter converter = new Xaml.CmToPixelConverter();
            
            double offSetX = converter.CmToPx(_viewWidth / 2 - _monitorWidth / 2);
            double offSetY = converter.CmToPx(_viewHeight / 2 - _monitorHeight / 2);

            double locationX = converter.CmToPx((Convert.ToDouble(source.X) / Prismatic.Right) * _monitorWidth);
            double locationY = converter.CmToPx((Convert.ToDouble(source.Y) / Prismatic.Bottom) * _monitorHeight);

            return new Point(locationX + offSetX, locationY + offSetY);
        }

        private System.Drawing.Point CalculatePosition(LedGroupViewModel ledGroup)
        {
            Xaml.CmToPixelConverter converter = new Xaml.CmToPixelConverter();

            double monitorWidth = converter.CmToPx(_monitorWidth);
            double monitorHeight = converter.CmToPx(_monitorHeight);

            double offSetX = converter.CmToPx(_viewWidth / 2 - _monitorWidth / 2);
            double offSetY = converter.CmToPx(_viewHeight / 2 - _monitorHeight / 2);

            int positionX = Convert.ToInt32(((ledGroup.Location.X - offSetX) / monitorWidth) * Prismatic.Right);
            int positionY = Convert.ToInt32(((ledGroup.Location.Y - offSetY) / monitorHeight) * Prismatic.Bottom);

            return new System.Drawing.Point(positionX, positionY);
        }

        private void ImportProfile(string profilePath)
        {
            ILightPackProfile profile = _profileService.Import(profilePath);

            profile.Leds.Join(_ledGroups, led => led.Index, led => led.Group, (import, group) => new { Import = import, Group = group }).ForEach(
                o =>
                {
                    Point location = CalculateLocation(o.Import.Position);

                    o.Group.Enabled = o.Import.IsEnabled;
                    o.Group.Location.X = location.X;
                    o.Group.Location.Y = location.Y;
                }
            );
        }

        private void ExportProfile(string profilePath)
        {
            LightPackProfile profile = new LightPackProfile(
                _ledGroups.Select(
                    ledGroup => new LightPackLed
                    (
                        ledGroup.Group,
                        ledGroup.Enabled,
                        CalculatePosition(ledGroup),
                        new System.Drawing.Size(50,50)
                    )
                )
            );

            _profileService.Export(profile, profilePath);
        }

        bool IDragDropManager.CanDrag(FrameworkElement AssociatedObject, object dataContext, out object dragData)
        {
            LedGroupViewModel viewModel = dataContext as LedGroupViewModel;

            if (viewModel != null)
            {
                dragData = viewModel;
                return true;
            }
            else
            {
                dragData = null;
                return false;
            }
        }

        bool IDragDropManager.CanDrop(FrameworkElement AssociatedObject, object dataContext, object dragData, out DragDropEffects effects)
        {
            LedGroupViewModel viewModel = dragData as LedGroupViewModel;

            if (viewModel != null)
            {
                effects = DragDropEffects.Move;
                return true;
            }
            else
            {
                effects = DragDropEffects.None;
                return false;
            }
        }

        void IDragDropManager.Drop(FrameworkElement AssociatedObject, object dataContext, object dragData, Point point)
        {
            LedGroupViewModel viewModel = dragData as LedGroupViewModel;

            if (viewModel != null)
            {
                viewModel.Location.X = point.X;
                viewModel.Location.Y = point.Y;
                viewModel.Enabled = !viewModel.Enabled;
            }
        }

        public ListCollectionView EnabledLedGroupCollection { get; private set; }

        public ListCollectionView DisabledLedGroupCollection { get; private set; }

        public ICommand ImportCommand
        {
            get { return _importCommand; }
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand; }
        }

        public IEnumerable<LedGroupViewModel> LedGroups 
        {
            get { return _ledGroups; }
        }

        public double ViewWidth
        {
            get { return _viewWidth; }
            set
            {
                if (value != _viewWidth)
                {
                    _viewWidth = value;

                    NotifyOfPropertyChange(() => ViewWidth);
                }
            }
        }

        public double ViewHeight
        {
            get { return _viewHeight; }
            set
            {
                if (value != _viewHeight)
                {
                    _viewHeight = value;

                    NotifyOfPropertyChange(() => ViewHeight);
                }
            }
        }

        public double ViewZoom
        {
            get { return _viewZoom; }
            set
            {
                if (value != _viewZoom)
                {
                    _viewZoom = value;

                    NotifyOfPropertyChange(() => ViewZoom);
                }
            }
        }

        public double MonitorWidth
        {
            get { return _monitorWidth; }
            set
            {
                if (value != _monitorWidth)
                {
                    _monitorWidth = value;

                    NotifyOfPropertyChange(() => MonitorWidth);
                }
            }
        }

        public double MonitorHeight
        {
            get { return _monitorHeight; }
            set
            {
                if (value != _monitorHeight)
                {
                    _monitorHeight = value;

                    NotifyOfPropertyChange(() => MonitorHeight);
                }
            }
        }
    }
}