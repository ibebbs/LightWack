using System;
using System.Collections.Generic;
using Caliburn.Micro;
using IEventAggregator = Reactive.EventAggregator.IEventAggregator;
using EventAggregator = Reactive.EventAggregator.EventAggregator;
using Ninject;
using Bebbs.LightWack.Services;

namespace Bebbs.LightWack
{
    public class AppBootstrapper : BootstrapperBase, IFactory
	{
		private IKernel _container;

		public AppBootstrapper()
		{
			Start();
		}

		protected override void Configure()
		{
            _container = new StandardKernel();
            _container.Load(new Module());

            _container.Bind<IFactory>().ToConstant(this).InSingletonScope();
		}

		protected override object GetInstance(Type service, string key)
		{
			var instance = _container.Get(service, key);
			if (instance != null)
				return instance;

			throw new InvalidOperationException("Could not locate any instances.");
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return _container.GetAll(service);
		}

		protected override void BuildUp(object instance)
		{
			_container.Inject(instance);
		}

        private void InitializeServices()
        {
            _container.GetAll<IInitializeAtStartup>().ForEach(service => service.Initialize());
        }

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
            InitializeServices();
			DisplayRootViewFor<IShell>();
		}

        public T Construct<T>()
        {
            return _container.Get<T>();
        }
    }
}