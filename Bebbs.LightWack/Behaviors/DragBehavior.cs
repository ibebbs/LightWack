using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Bebbs.LightWack.Behaviors
{
    public class DragBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty DragDropManagerProperty = DependencyProperty.Register("DragDropManager", typeof(IDragDropManager), typeof(DragBehavior), new PropertyMetadata(null));
        
        private bool _mouseDown;

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            if (_mouseDown && DragDropManager != null)
            {
                FrameworkElement dragSource = AssociatedObject;
                object dataContext = AssociatedObject.DataContext;
                object dragData;

                if (DragDropManager.CanDrag(AssociatedObject, dataContext, out dragData))
                {
                    dragData = dragData ?? dataContext ?? AssociatedObject;

                    DataObject dataObject = new DataObject();
                    dataObject.SetData(typeof(object), dragData);

                    DragDrop.DoDragDrop(AssociatedObject, dataObject, DragDropEffects.Move);
                }
            }
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += MouseLeftButtonUp;
            AssociatedObject.MouseLeave += MouseLeave;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= MouseLeftButtonUp;
            AssociatedObject.MouseLeave -= MouseLeave;

            base.OnDetaching();
        }

        public IDragDropManager DragDropManager
        {
            get { return (IDragDropManager)GetValue(DragDropManagerProperty); }
            set { SetValue(DragDropManagerProperty, value); }
        }
    }
}
