using System.Windows;
using System.Windows.Interactivity;

namespace Bebbs.LightWack.Behaviors
{
    public class DropBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty DragDropManagerProperty = DependencyProperty.Register("DragDropManager", typeof(IDragDropManager), typeof(DropBehavior), new PropertyMetadata(null));

        private void Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(object)) && DragDropManager != null)
            {
                object dataContext = AssociatedObject.DataContext;
                object dragData = e.Data.GetData(typeof(object));
                DragDropEffects effects;

                if (DragDropManager.CanDrop(AssociatedObject, dataContext, dragData, out effects))
                {
                    DragDropManager.Drop(AssociatedObject, dataContext, dragData, e.GetPosition(AssociatedObject));
                }

                e.Handled = true;
            }
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(object)) && DragDropManager != null)
            {
                object dataContext = AssociatedObject.DataContext;
                object dragData = e.Data.GetData(typeof(object));
                DragDropEffects effects;

                if (DragDropManager.CanDrop(AssociatedObject, dataContext, dragData, out effects))
                {
                    e.Effects = effects;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }

                e.Handled = true;
            }
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += DragEnter;
            AssociatedObject.DragOver += DragOver;
            AssociatedObject.DragLeave += DragLeave;
            AssociatedObject.Drop += Drop;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AllowDrop = false;
            AssociatedObject.DragEnter -= DragEnter;
            AssociatedObject.DragOver -= DragOver;
            AssociatedObject.DragLeave -= DragLeave;
            AssociatedObject.Drop -= Drop;

            base.OnDetaching();
        }

        public IDragDropManager DragDropManager
        {
            get { return (IDragDropManager)GetValue(DragDropManagerProperty); }
            set { SetValue(DragDropManagerProperty, value); }
        }
    }
}
