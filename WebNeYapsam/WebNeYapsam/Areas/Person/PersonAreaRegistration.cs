using System.Web.Mvc;

namespace WebNeYapsam.Areas.Person
{
    public class PersonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Person";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Person_default",
                "Person/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "WebNeYapsam.Areas.Person.Controllers" }
            );
        }
    }
}