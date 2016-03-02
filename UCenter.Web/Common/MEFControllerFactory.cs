using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UCenter.Web
{
    [Export]
    public class MEFControllerFactory : DefaultControllerFactory
    {
        private readonly ExportProvider exportProvider; // This container will work like an IOC container

        [ImportingConstructor]
        public MEFControllerFactory(ExportProvider exportProvider)
        {
            this.exportProvider = exportProvider;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            Lazy<object, object> export = exportProvider.GetExports(controllerType, null, null).FirstOrDefault();

            //here if the controller object is not found (resulted as null) we can go ahead and get the default controller.
            //This means that in order to get the Controller we have to Export it first which will be shown later in this post
            return export == null ? base.GetControllerInstance(requestContext, controllerType) : (IController)export.Value;
        }

        public override void ReleaseController(IController controller)
        {
            //this is were the controller gets disposed
            ((IDisposable)controller).Dispose();
        }
    }
}