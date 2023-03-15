using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using Arkenstone.Entities;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.Extensions.Options;

namespace Arkenstone
{
    public class Startup
    {
        public static readonly string PolicyAllOrigin = "AllowAllOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            Arkenstone.Logic.Logs.ClassLog.writeLog(nameof(Startup) + "-" + nameof(ConfigureServices) + " => " + "BOOTING....");

            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("sessiondataprotection/"));

            services.AddDistributedMySqlCache(options =>
            {
                options.ConnectionString = System.Environment.GetEnvironmentVariable("DB_SESSION_connectionstring");
                options.TableName = "webusersessions";
                options.SchemaName = "eveminingfleetsession";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.IOTimeout = TimeSpan.FromDays(7);
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.MaxAge = TimeSpan.FromDays(7);
            });

            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            Console.WriteLine($"Connection string: {_dbConnectionString}");
            services.AddDbContext<ArkenstoneContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString), opts => opts.EnableRetryOnFailure(3).CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds))
                    );


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = System.Environment.GetEnvironmentVariable("TokenIssuer"),
                            ValidAudience = System.Environment.GetEnvironmentVariable("TokenIssuer"),
                            IssuerSigningKey = JwtSecurityKey.Create(System.Environment.GetEnvironmentVariable("TokenSecretKey"))
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Member", policy => policy.RequireClaim("MembershipId"));
                options.AddPolicy("ITWebMedia", policy => policy.RequireClaim("ItWebMediaMember"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: PolicyAllOrigin,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    });
            });

            services.AddSwaggerGen();
            services.AddControllers();

            //services.AddControllersWithViews()
            //    .AddNewtonsoftJson(options =>
            //        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //    );

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                if (env.IsDevelopment())
                    c.SupportedSubmitMethods(new SubmitMethod[] { });
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            FluentScheduler.JobManager.Initialize(new _BackgroundTask());
        }
    }
}