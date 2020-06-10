using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraceChapelLibraryWebApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using AutoMapper;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Services;
using Microsoft.AspNetCore.Http;
using GraceChapelLibraryWebApp.Core.Scheduler;
using GraceChapelLibraryWebApp.Core.Repositories;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using GraceChapelLibraryWebApp.Core.Templates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GraceChapelLibraryWebApp
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
            services.Configure<ApplicationSettings>((Configuration.GetSection("ApplicationSettings")));

            services.AddDbContext<BookLibraryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BookLibraryDatabase")));

            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
                .AddEntityFrameworkStores<BookLibraryContext>();


            NewMethod((services));
            services.AddControllers();



            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

            });

            services.AddAutoMapper(typeof(Startup));

            // add authentication 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "http://localhost:63568/",
                        ValidAudience = "http://localhost:63568/",
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:Jwt_Secret"]))
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {

                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Grace Chapel Library",
                    Description = "Grace Chapel Library",
                    TermsOfService = new System.Uri("https://google.com"),
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Admin",
                        Email = string.Empty,
                        Url = new System.Uri("https://google.com")

                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "License",
                        Url = new System.Uri("https://google.com")
                    }
                }); ;
            });
        }

        private static void NewMethod(IServiceCollection services)
        {
            // add the repository wrapper here
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            // service for the mail client 
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<IEmailTemplate, EmailTemplate>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // schedule tasks
            services.AddSingleton<IHostedService, ScheduleTaskChangeBorrowerStatus>();
            services.AddSingleton<IHostedService, ScheduleTaskMonthlyReportForAdmin>();
            services.AddSingleton<IHostedService, ScheduleTaskWeeklyReminderForOverdue>();
            services.AddSingleton<IHostedService, ScheduleTaskPreExpiryReminder>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.DocumentTitle = $"GraceChapel API Documentation";
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "GraceChapel Book Library V1");
                option.RoutePrefix = string.Empty;
                option.DocExpansion(DocExpansion.None);
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseCors("EnableCORS");
            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=index}/{id?}");
            });

        }
    }
}