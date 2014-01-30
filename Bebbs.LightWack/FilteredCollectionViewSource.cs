using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bebbs.LightWack
{
    public interface IFilter
    {
        bool Include(object source);

        string PropertyName { get; }
    }

    public class Filter : IFilter
    {
        private Predicate<object> _predicate;

        public Filter(string propertyName, Predicate<object> predicate)
        {
            PropertyName = propertyName;
            _predicate = predicate;
        }

        public bool Include(object source)
        {
            return _predicate(source);
        }

        public string PropertyName { get; private set; }
    }


    public class FilteredCollectionViewSource : CollectionViewSource
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IFilter), typeof(FilteredCollectionViewSource), new PropertyMetadata(null, FilterPropertyChanged));

        private static void FilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FilteredCollectionViewSource collectionViewSource = (sender as FilteredCollectionViewSource);

            if (collectionViewSource != null)
            {
                collectionViewSource.FilterChanged();
            }
        }

        private void FilterItem(object sender, FilterEventArgs e)
        {
            IFilter filter = Filter;

            e.Accepted = filter == null || filter.Include(e.Item);
        }

        protected override void OnSourceChanged(object oldSource, object newSource)
        {
            base.OnSourceChanged(oldSource, newSource);

            FilterChanged();
        }

        private void FilterChanged()
        {
            ListCollectionView view = CollectionViewSource.GetDefaultView(this) as ListCollectionView;

            if (view != null)
            {
                view.Filter = null;
                view.LiveFilteringProperties.Clear();

                IFilter filter = Filter;

                if (filter != null)
                {
                    view.Filter += filter.Include;
                    view.IsLiveFiltering = true;
                    view.LiveFilteringProperties.Add(filter.PropertyName);
                }

                view.Refresh();
            }
        }

        public new IFilter Filter
        {
            get { return (IFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
    }
}
