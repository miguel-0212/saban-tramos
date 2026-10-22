using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mantenimiento_Liquidaciones
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // La identidad de desarrollo (Application_PostAuthenticateRequest) solo tiene el claim Name;
            // sin esto @Html.AntiForgeryToken() lanza InvalidOperationException (falta nameidentifier).
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(RequiredAttributeAdapter));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsLocal) // Verifica si es entorno de desarrollo
            {
                // Crea una nueva identidad con el nombre de usuario deseado
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "CONTOSO\\gsubirana"), // Cambia "TuNombreDeUsuarioDeseado" al valor que necesites
                    // Puedes agregar más claims si es necesario
                };
                var identity = new ClaimsIdentity(claims, "Desarrollo");
                var principal = new ClaimsPrincipal(identity);

                // Establece la identidad del usuario en el contexto actual
                HttpContext.Current.User = principal;
            }
        }
    }

}
