using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Services;

//using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Mvc;

namespace BattlefieldCompetitivePortal.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected User CurrentUser => getCurrentUser();

        private async User getCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);
                return new UserService().GetUserById(userId);
            }
            return null;
        }

        protected ActionResult RequiredRole(UserRole role)
        {
            if (CurrentUser?.Role != role)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
