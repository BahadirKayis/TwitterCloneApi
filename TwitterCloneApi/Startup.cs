using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterCloneApi.Hubss;
using TwitterCloneApi.Models;

namespace TwitterCloneApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
          

            string connectionString = "Data Source=127.0.0.1; User Id=bahadir; Password=Yasak123;  Database=twitter;" + "MultipleActiveResultSets=True";
        // string connectionString = "Data Source=127.0.0.1\\SQLSAMBA; User Id=sa; Password=yasak123;  Database=TwitterClone;" + "MultipleActiveResultSets=True";

           

            services.AddSwaggerGen(c =>

            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Twitter Clone", Version = "v1" });
            });

            services.AddControllers().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddDbContext<TwitterCloneContext>(x => x.UseSqlServer(connectionString));
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
            services.AddMvc();
            services.AddMemoryCache();

            services.AddControllers();
            services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyMethod()));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwitterClone v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TwitterClone v1"));
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NewTweetHub>("/newTweetHub");
                endpoints.MapSwagger();
            });
        }
    }
}
