using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using SqlSugar.IOC;
using Pro.Core.WebApi.Common;
using System.IO;

namespace Pro.Core.WebApi
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

            //注册Swagger
            services.AddSwaggerGen(u =>
            {
                u.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "Ver:1.0.0",//版本
                    Title = "学生管理系统",//标题
                    Description = "后台管理系统：包括学生信息、部门管理等。",//描述
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "UserName",
                        Email = "***@hotmail.com"
                    }
                });
            });

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connString = config.GetConnectionString("DefaultConnection");

            services.AddControllers();
            services.AddSqlSugar(new IocConfig()
            {
                ConnectionString = connString,
                DbType = IocDbType.SqlServer,
                IsAutoCloseConnection = true
            });
            services.AddIoc(this, "Pro.Core.BLL", it => it.Name.Contains("StudentBLL"));
            services.AddIoc(this, "Pro.Core.WebApi", it => it.Name.Contains("Controller"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //启用Swagger中间件
            app.UseSwagger();
            //配置SwaggerUI
            app.UseSwaggerUI(u =>
            {
                u.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI_v1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
