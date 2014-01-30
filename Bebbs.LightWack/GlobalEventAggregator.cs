using Reactive.EventAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack
{
    public interface IGlobalEventAggregator : IEventAggregator
    {

    }

    public class GlobalEventAggregator : EventAggregator, IGlobalEventAggregator
    {
    }
}
