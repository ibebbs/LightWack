using Bebbs.LightWack.Messages;
using LightpackNet;
using LightpackNet.Answers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack.Services
{
    public interface ILightPackService : IInitializeAtStartup
    {

    }

    internal class LightPackService : ILightPackService
    {
        private readonly IGlobalEventAggregator _eventAggregator;

        private Lightpack _lightPack;
        private IDisposable _subscription;

        public LightPackService(IGlobalEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        private byte[] LedMap
        {
            get
            {
                return Enumerable.Range(0, 255).Select(value => Convert.ToByte(value)).ToArray();
            }
        }

        private void Process(LedStateChanged message)
        {
            if (message.Lit)
            {
                CommonAnswer answer = _lightPack.SetColor(message.Led, message.Color);

                System.Diagnostics.Debug.WriteLine(answer.ToString());
            }
            else
            {
                _lightPack.SetColor(message.Led, Color.Black);
            }
        }

        public void Initialize()
        {
            _lightPack = new Lightpack("127.0.0.1", 3636, LedMap);
            _lightPack.Connect();
            _lightPack.Lock();

            _subscription = _eventAggregator.GetEvent<LedStateChanged>().Subscribe(Process);
        }
    }
}
