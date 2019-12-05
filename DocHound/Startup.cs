using System;
using DocHound.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocHound.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DocHound
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            SettingsHelper.SetGlobalConfiguration(configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // set up and configure Authentication - make sure to call .UseAuthentication()
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

                .AddCookie(o =>
                {
                    o.LoginPath = "/___account___/signin";
                    o.LogoutPath = "/___account___/signout";
                    o.SlidingExpiration = true;
                    o.ExpireTimeSpan = new TimeSpan(2, 0, 0, 0);
                });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("account", "___{controller}___/{action}/{id?}", new {controller = "Account", action = "Signin"});
                routes.MapRoute("fileproxy", "___FileProxy___", new {controller = "Docs", action = nameof(DocsController.FileProxy)});
                routes.MapRoute("docs", "{*url}", new {controller = "Docs", action = nameof(DocsController.Topic)});
            });
        }
    }
}