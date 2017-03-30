using CallCenter.Data;
using CallCenter.Model.Abstract;
using CallCenter.Model.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CallCenter.UI.App_Start
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<ICallCenterStorage>().To<CallCenterStorage>().WithConstructorArgument("CallCenterDB");
            kernel.Bind<ICallCenterService>().To<CallCenterService>();
        }
    }
}