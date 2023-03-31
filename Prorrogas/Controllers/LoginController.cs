using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Prorrogas.Models;
using System.Security.Claims;

namespace Prorrogas.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            var user = User.Identity.Name;
            if (string.IsNullOrEmpty(user))
            {
                return View();
            }
            else
            {
                ProrrogasEntities db = new ProrrogasEntities();
                var usuarioLogueado = db.Usuario.SingleOrDefault(x => x.usuario1 == User.Identity.Name && x.estado == true);
                if (usuarioLogueado !=  null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index(string inputUser, string inputPassword)
        {

            ProrrogasEntities db = new ProrrogasEntities();
            Status s = new Status();
            if (ModelState.IsValid)
            {
                var usuarioHabilitado = db.Usuario.SingleOrDefault(x => x.usuario1 == inputUser && x.contraseña.Equals(inputPassword) && x.estado == true);
                if (usuarioHabilitado != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                    identity.AddClaim(new Claim(ClaimTypes.Name, inputUser));
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = true;
                    authenticationManager.SignIn(props, identity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.UsuarioInvalido = 1;
                }
            }
            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Logout()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}