using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthEquity.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using HealthEquity.Data;
using Microsoft.EntityFrameworkCore;
using HealthEquity.Data.Mapper;
using Microsoft.OpenApi.Models;
using static AutoMapper.Internal.ExpressionFactory;
using HealthEquity.Models;

namespace HealthEquity
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

           
            services.AddDbContext<HealthEquityDbContext>(options =>
                          options.UseSqlServer(Configuration.GetConnectionString("healthEquityDatabaseConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                 new OpenApiInfo { Title = "my api", Version = "v1" });
            });
            //var mappingConfig = new MapperConfiguration(mc =>
            //{
            //    mc.AddProfile(new MemberProfile());
            //});
            //IMapper mapper = mappingConfig.CreateMapper();
            //services.AddSingleton(mapper);

           
            services.AddAutoMapper(typeof(Startup));

            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddScoped<IRepositoryMember, RepositoryMember>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });
            app.UseHttpsRedirection();
            using(var srvc = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = srvc.ServiceProvider.GetService<HealthEquityDbContext>();
                context.Database.Migrate();
            }
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseMvc();
        }
    }
}
