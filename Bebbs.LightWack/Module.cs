using Bebbs.LightWack.Services;
using Caliburn.Micro;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bebbs.LightWack
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            Bind<IGlobalEventAggregator>().To<GlobalEventAggregator>().InSingletonScope();
            Bind<ILightPackProfileService>().To<LightPackProfileService>().InSingletonScope();
            Bind<ILightPackService, IInitializeAtStartup>().To<LightPackService>().InSingletonScope();
            
            Bind<IShell>().To<ViewModels.ShellViewModel>();
            Bind<ViewModels.LedGroupViewModel>().ToSelf();
            Bind<ViewModels.LedViewModel>().ToSelf();
        }
    }
}
