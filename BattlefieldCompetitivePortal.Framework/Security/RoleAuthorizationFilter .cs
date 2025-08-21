using BattlefieldCompetitivePortal.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Mvc;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BattlefieldCompetitivePortal.Framework.Security
{
    public class RoleAuthorizationFilter : ActionFilterAttribute
    {
        private readonly UserRole[] _allowedroles;

        public RoleAuthorizationFilter(params UserRole[] allowedRoles)
        {
            _allowedroles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRole = user.FindFirst("Role");
            if (userRole == null || !_allowedroles.Contains((UserRole)int.Parse(userRole.Value)))
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
