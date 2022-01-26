using HermesChatTeamB_v3.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;



namespace HermesChatTeamB_v3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();
            services.AddControllersWithViews();

            //// Register IUserTracker used by ChatHub.
            services.AddSingleton(typeof(IUserTracker), typeof(UserTracker));

            services.AddMvc();
            services.AddSignalR();
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //// Redirect the request to HTTPS, if its Http.
            ////app.UseRewriter(new RewriteOptions().AddRedirectToHttps()); ////301, 44346

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();

            
            //// Use - SignalR & let it know to intercept and map any request having chatHub.
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<ChatHub>("chatHub");
            //});


            /*app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });*/

        app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("chatHub");

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /*public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        //// Use - SignalR & let it know to intercept and map any request having chatHub.
        app.UseSignalR(routes =>
        {
            routes.MapHub<ChatHub>("chatHub");
        });

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        });*/
    }
}
