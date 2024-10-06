using Microsoft.EntityFrameworkCore;
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

            // To preserve the original property names
            services.AddControllers().AddJsonOptions(options =>
                    options.JsonSerializerOptions.PropertyNamingPolicy = null);

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