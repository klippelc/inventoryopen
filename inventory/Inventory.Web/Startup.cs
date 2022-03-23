using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartup(typeof(Inventory.Web.Startup))]

namespace Inventory.Web
{
    public class Startup

    {
        public void Configuration(IAppBuilder app)

        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()

            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

                LoginPath = new PathString("/Account/Login"),

                CookieSecure = CookieSecureOption.Always,

                CookieHttpOnly = true,

                CookieName = ".ASSETS",

                CookieSameSite = SameSiteMode.None,

                ExpireTimeSpan = TimeSpan.FromMinutes(20),

                LogoutPath = new PathString("/Account/Logoff")
            });
        }
    }
}