namespace GF.UCenter.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The route configuration
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Register route configuration
        /// </summary>
        /// <param name="routes">The route collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}