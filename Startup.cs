/*namespace WebAppDBMS
{
    public class Startup
    {
    }
}*/

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using WebAppDBMS.Models;

namespace DBMSWebApp
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
            services.AddDbContext<DBContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();
        }


        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Databases}/{action=Index}/{id?}");

            });
        }
    }
}