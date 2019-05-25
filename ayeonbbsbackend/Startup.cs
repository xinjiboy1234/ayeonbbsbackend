using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ayeonbbsbackend.ModelDbContext;
using ayeonbbsbackend.Services;
using ayeonbbsbackend.Utils;

namespace ayeonbbsbackend
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region 服务注册

            services.AddScoped<UserService>();
            services.AddScoped<FirstCategoryService>();
            services.AddScoped<SecondCategoryService>();
            services.AddScoped<PostService>();
            services.AddScoped<ReplyService>();
            services.AddScoped<PostGoodService>();
            services.AddScoped<ReplyGoodService>();

            #endregion

            #region 跨域
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200").AllowAnyHeader();
                    });
            });
            #endregion

            #region JWT 验证

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //是否验证Issuer
                        ValidateAudience = true, //是否验证Audience
                        ValidateLifetime = true, // 有过期时间
                        ValidateIssuerSigningKey = true, // 是否验证IssuerKey 
                        ValidAudience = "bbs.soarwhale.com",
                        ValidIssuer = "bbs.soarwhale.com", // 这两个必须一样
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["mytoken"]))
                    };
                });

            #endregion

            #region 数据库链接
            services.AddDbContext<DataContext>(option => option.UseSqlite(Configuration.GetConnectionString("ConnSqlite")));

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //静态文件处理
                var dir = Configuration["imguploadpath"];
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), dir)),
                    RequestPath = "/uploadimg"
                });
                app.UseCors("AllowSpecificOrigin");//跨域
                app.UseAuthentication(); // 启用 JWT 验证
            }
            else
            {
                //静态文件处理
                var dir = Configuration["imguploadpath"];
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), dir)),
                    RequestPath = "/uploadimg"
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                app.UseAuthentication();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
