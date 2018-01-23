using DocHound.Controllers;
using DocHound.Models.Docs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocHound.Interfaces;

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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("fileproxy", "___FileProxy___", new {controller = "Docs", action = nameof(DocsController.FileProxy)});
                routes.MapRoute("docs", "{topicName=index}", new {controller = "Docs", action = nameof(DocsController.Topic)});
            });
        }
    }
}