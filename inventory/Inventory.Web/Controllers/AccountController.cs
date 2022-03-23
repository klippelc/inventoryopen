using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Inventory.Web.AdAuthentication;
using Inventory.Web.Common;
using Inventory.Web.Logic;
using Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory.Web.Controllers
{
    public class AccountController : AssetBaseMvcController
    {
        public AccountController(IAssetLogic assetLogic, IUserLogic userLogic)
        {
            AssetLogic = assetLogic;
            UserLogic = userLogic;
            AssetType = "Login";
            ViewBag.AssetType = AssetType;
        }

        [AllowAnonymous]
        public ActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVM vm)
        {
            var user = await UserLogic.GetUserRoles(vm.Username);

            if (user is null)
            {
                ModelState.Remove("UserName");
                vm.Username = vm.Username.RemoveSpaces();
                ModelState.AddModelError(nameof(vm.Password), "The User is not found");
            }
            else if (user.Active == false)
            {
                ModelState.AddModelError(nameof(vm.Password), "The User is not active");
            }
            else if (user.UserRoles == null || user.UserRoles.Count() == 0)
            {
                ModelState.AddModelError(nameof(vm.Password), "The User has no roles");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    var authHost = new ADAuthenticationInternalSoapClient("ADAuthenticationInternalSoap");
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    ////Un-Comment/Comment if you want to use BC Identity
                    //
                    //var authResult = authHost.SubmitUserAuthentication(vm.Username, vm.Password, AdApplicationName, vm.IpAddress.Equals("::1") ? "127.0.0.1" : vm.IpAddress);

                    //if (authResult.IsAuthenticated)
                    //{
                    ////To Here

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserId.ToString())),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(CustomClaimTypes.LastName, user.LastName ?? ""),
                            new Claim(CustomClaimTypes.FirstName, user.FirstName ?? ""),
                            new Claim(CustomClaimTypes.Initials, user.Initials ?? ""),
                            new Claim(CustomClaimTypes.LocationId, Convert.ToString(user.LocationId))
                        };

                    if (user.UserRoles.Any())
                    {
                        foreach (var role in user.UserRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Name));
                        }
                    }

                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    var ctx = HttpContext.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    //Update Last Login Date
                    await UserLogic.UpdateLastLogin(user.UserId);

                    if (!string.IsNullOrEmpty(vm.ReturnUrl))
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ////Un-Comment if you want to use BC Identity
                    //
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError(nameof(vm.Password), "" + authResult.Exception);
                    //    return View(vm);
                    //}
                    ///////////To Here
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(nameof(vm.Password), "" + e?.Message + " " + e.InnerException?.Message + " " + e.InnerException?.InnerException);
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult Logoff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}