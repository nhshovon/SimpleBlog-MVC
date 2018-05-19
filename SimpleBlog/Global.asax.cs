using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SimpleBlog.Models;
using WebMatrix.WebData;

namespace SimpleBlog
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            SimpleMembershipInitializer();
        }

        private void SimpleMembershipInitializer()
        {
            Blog_Db_Entities entities = new Blog_Db_Entities();

            if (!entities.Database.Exists())
            {
                ((IObjectContextAdapter)entities).ObjectContext.CreateDatabase();
            }

            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            InitializeRoles();
        }

        private void InitializeRoles()
        {
            if (!Roles.RoleExists("Developer"))
            {
                Roles.CreateRole("Developer");
            }
            if (!Roles.RoleExists("Admin"))
            {
                Roles.CreateRole("Admin");
            }
            if (!Roles.RoleExists("User"))
            {
                Roles.CreateRole("User");
            }
        }
    }
}
