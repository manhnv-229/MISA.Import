using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Services;
using MISA.ImportDemo.Infrastructure.Data;
using MISA.ImportDemo.Infrastructure.Data.Repositories;
using MISA.ImportDemo.Infrastructure.DatabaseContext;
using MISA.ImportDemo.Interfaces;
using MISA.ImportDemo.Middleware;
using MISA.ImportDemo.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace MISA.ImportDemo
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
            EfDbContext.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            DatabaseContext.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddMemoryCache();
            services.AddDbContext<EfDbContext>(options => options
                .UseMySql(EfDbContext.ConnectionString,
                    mysqlOptions =>
                        mysqlOptions.ServerVersion(
                            new ServerVersion(new Version(10, 4, 6), ServerType.MariaDb))
                        .EnableRetryOnFailure())
                );

            services.AddCors();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                })
             .ConfigureApiBehaviorOptions(options =>
             {
                 options.SuppressModelStateInvalidFilter = true;
             });

            services.AddDistributedMemoryCache();
            services.AddSession();

            // cho lower case hết nhìn cho nó chuyên nghiệp:
            services.AddRouting(options => options.LowercaseUrls = true);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MISA Import Service - Nhập khẩu, Xuất khẩu dữ liệu", Version = "v1" });
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                c.IncludeXmlComments(xmlFile);
            });

            // Cấu hình để có thể đọc và tải File tệp mẫu:
            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "FileTemplate")));

            // Config DI:
            // 1. Base:
            services.AddScoped(typeof(IBaseEntityService<>), typeof(BaseEntityService<>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(EfBaseRepository<>));
            services.AddScoped(typeof(IEfBaseRepository<>), typeof(EfBaseRepository<>));
            services.AddScoped<IEntityRepository, BaseEntityRepository>();
            services.AddScoped<IBaseImportRepository, BaseImportRepository>();

            // 3. Nhập khẩu Nhân viên:
            services.AddScoped<IImportEmployeeService, ImportEmployeeService>();
            services.AddScoped<IImportEmployeeRepository, ImportEmployeeRepository>();

            // 4. Khác
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MISA iVAN Service version 1");
                c.RoutePrefix = "swagger";
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
