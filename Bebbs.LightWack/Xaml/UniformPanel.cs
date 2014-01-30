using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bebbs.LightWack.Xaml
{
    public class UniformPanel : UniformGrid
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformPanel), new PropertyMetadata(Orientation.Horizontal));

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                Columns = VisualChildrenCount;
            }
            else
            {
                Rows = VisualChildrenCount;
            }

            return base.ArrangeOverride(arrangeSize);
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}
