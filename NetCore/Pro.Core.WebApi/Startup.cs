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

            //ע��Swagger
            services.AddSwaggerGen(u =>
            {
                u.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "Ver:1.0.0",//�汾
                    Title = "ѧ������ϵͳ",//����
                    Description = "��̨����ϵͳ������ѧ����Ϣ�����Ź���ȡ�",//����
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

            //����Swagger�м��
            app.UseSwagger();
            //����SwaggerUI
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
