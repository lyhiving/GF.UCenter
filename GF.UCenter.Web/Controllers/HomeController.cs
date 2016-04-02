namespace GF.UCenter.Web.Controllers
{
    using System.ComponentModel.Composition;
    using System.Web.Mvc;

    /// <summary>
    ///     Home page controller
    /// </summary>
    [Export]
    public class HomeController : Controller
    {
        /// <summary>
        ///     Get the index page
        /// </summary>
        /// <returns>The index page view</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        ///     Get the about page view.
        /// </summary>
        /// <returns>The about page view.</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return this.View();
        }

        /// <summary>
        ///     Get the contact page view.
        /// </summary>
        /// <returns>The contact page view.</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}