
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploreCalifornia.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ExploreCalifornia
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<FormattingService>();
            services.AddTransient<FeatureToggles>(
                x => new FeatureToggles
                {
                    DeveloperExceptions = configuration.GetValue<bool>("FeatureToggle:DeveloperException")
                }
                );
            services.AddDbContext<BlogDataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("BlogDataContext");
                options.UseSqlServer(connectionString);
            });
            services.AddDbContext<IdentityDataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("IdentityDataContext");
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<IdentityUser,IdentityRole>()
                 .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
            ,
            FeatureToggles feature)
        {
            app.UseExceptionHandler("/error.html");
            /// this will only work with production pages soo go on properties then debug 
            /// and change envi. var value to production
            /// 
           /* if (configuration.GetValue<bool>("FeatureToggles:DeveloperExceptions"))
            {
                app.UseDeveloperExceptionPage();
            }*/
            if (feature.DeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseRouting();
            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/hellow", async context =>
                {
                    await context.Response.WriteAsync("Hello World! It's N1ki Brahmbhatt");
                });
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World! It's Vishal Brahmbhatt");
                });
            });*/





            /// for error handling checking

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains("invalid"))
                    throw new Exception("Error!");

                await next();

            });///instead of use this use this one---> down

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseFileServer(); //for directly use of wwwroot folder. . .
        }
    }
}
