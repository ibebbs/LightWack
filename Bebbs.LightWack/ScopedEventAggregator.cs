using Reactive.EventAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack
{
    public interface IScopedEventAggregator : IEventAggregator
    {

    }

    public class ScopedEventAggregator : EventAggregator, IScopedEventAggregator
    {
    }
}
