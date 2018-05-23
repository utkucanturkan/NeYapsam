using System.Web.Mvc;

namespace WebNeYapsam.Areas.Panel
{
    public class PanelAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Panel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Panel_default_1",
                "Panel/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional },
                new[] { "WebNeYapsam.Areas.Panel.Controllers" }
            );

            context.MapRoute(
                "Panel_default_",
                "Panel/User/Page/{name}",
                new { name = UrlParameter.Optional },
                new[] { "WebNeYapsam.Areas.Panel.Controllers" }
            );

            context.MapRoute(
                "Panel_default",
                "Panel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "WebNeYapsam.Areas.Panel.Controllers" }
            );
        }
    }
}