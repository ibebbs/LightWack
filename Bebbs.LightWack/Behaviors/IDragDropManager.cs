using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bebbs.LightWack.Behaviors
{
    public interface IDragDropManager
    {
        bool CanDrag(FrameworkElement AssociatedObject, object dataContext, out object dragData);

        bool CanDrop(FrameworkElement AssociatedObject, object dataContext, object dragData, out DragDropEffects effects);

        void Drop(FrameworkElement AssociatedObject, object dataContext, object dragData, Point point);
    }
}
