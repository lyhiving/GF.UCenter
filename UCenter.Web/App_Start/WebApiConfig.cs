using System.Web.Http;

namespace UCenter.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //var container = new UnityContainer();
            //container.RegisterType<IHttpController, AccountController>(new HierarchicalLifetimeManager());
            //container.RegisterType<IAccountHandler, AccountHandler>(new HierarchicalLifetimeManager());
            //config.DependencyResolver = new UnityResolver(container);
        }
    }
}
