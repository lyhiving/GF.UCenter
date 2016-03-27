using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http.Dependencies;

namespace GF.UCenter.Web
{
    public class MefDependencyResolver : IDependencyResolver
    {
        private readonly ExportProvider exportProvider;

        public MefDependencyResolver(ExportProvider exportProvider)
        {
            this.exportProvider = exportProvider;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {

        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var contractName = AttributedModelServices.GetContractName(serviceType);
            var export = this.exportProvider.GetExportedValueOrDefault<object>(contractName);
            return export;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            var exports = this.exportProvider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
            return exports;
        }
    }
}