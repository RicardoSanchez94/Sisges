using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SistemaGestion.Models;
using SistemaGestion.Models.Repositorios;
using SistemaGestion.Models.ViewModels;

namespace SistemaGestion.Models.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        SisGesEntities3 db;

        public AuthorizeRolesAttribute()
        {
            db = new SisGesEntities3();

        }
        private readonly string[] userAssignedRoles;
        public AuthorizeRolesAttribute(params string[] roles)
        {
            this.userAssignedRoles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;

            AdminUsuarios ADU = new AdminUsuarios();
            foreach (var roles in userAssignedRoles)
            {
                var loginname = JsonConvert.DeserializeObject<UserLoginView>(HttpContext.Current.User.Identity.Name);
                //authorize = ADU.IsUserInRole(HttpContext.Current.User.Identity.Name, roles);
                authorize = ADU.IsUserInRole(loginname.LoginName, roles);
                if (authorize)
                    return authorize;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Account/UnAuthorized");
        }
    }
}