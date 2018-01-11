using DocHound.Models.Docs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocHound
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var gitHubProject = Configuration["GitHubProject"];
            if (string.IsNullOrEmpty(gitHubProject))
            {
                TopicViewModel.MasterUrl = Configuration["MasterUrl"];
                TopicViewModel.MasterUrlRaw = TopicViewModel.MasterUrl.Replace("https://github.com", "https://raw.githubusercontent.com/") + "/master/";
            }
            else
            {
                TopicViewModel.MasterUrl = "https://github.com/" + gitHubProject;
                TopicViewModel.MasterUrlRaw = "https://raw.githubusercontent.com/" + gitHubProject + "/master/";
            }
        }

        public IConfiguration Configuration { get; }

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
                routes.MapRoute("docs", "{topicName=index}", new {controller = "Docs", action = "Topic"});
            });
        }
    }
}