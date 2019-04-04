using CmsShop.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CmsShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            if (User == null)
            {
                return;
            }

            // pobranie nazwy użytkownika 
            string username = Context.User.Identity.Name;

            // deklaracja tablicy z rolami
            string[] roles = null;

            using(Db db = new Db())
            {
                // pobranie danych dla uzytkownika z bazy aby pobrać role
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == username);
                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();
            }

            // tworzenie IPrincipal object
            IIdentity userIdentity = new GenericIdentity(username);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // aktualizacja Context.User
            Context.User = newUserObj;

        }
    }
}
