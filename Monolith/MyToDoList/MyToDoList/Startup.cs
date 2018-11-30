using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using MyToDoList.Database;
using MyToDoList.Models;
using MyToDoList.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Swashbuckle;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyToDoList.Filter;
using MyToDoList.Services;
using SendGrid;

namespace MyToDoList
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
            var serviceProvider = services.BuildServiceProvider();
            var env = serviceProvider.GetService<IHostingEnvironment>();
            services.AddMvc(options => 
            {
                options.Filters.Add<UserInjectionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            string connectionString = Configuration.GetConnectionString("MyToDoListDB");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            
            services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["Issuer"],
                    ValidAudience = Configuration["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecret"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "MyToDoList.API", Version = "v1" });
            });

            CreateObjectMappings();

            services.AddSingleton<IMailService, SendGridMailService>();
            services.AddSingleton<SendGridClient>(factory =>
            {
                string sendGridApiKey = Configuration["SendGridApiKey"];
                return new SendGridClient(new SendGridClientOptions()
                {
                    ApiKey = sendGridApiKey
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors(config =>
            {
                config.AllowAnyHeader();
                config.AllowAnyMethod();
                config.AllowAnyOrigin();
                config.AllowCredentials();
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyToDoList.API v1");
            });
            app.UseMvc();
        }

        private void CreateObjectMappings()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<ApplicationUser, UserSignUpDTO>(MemberList.Destination);
                config.CreateMap<UserSignUpDTO, ApplicationUser>(MemberList.Source);
                config.CreateMap<ApplicationUser, UserSignInDTO>(MemberList.Destination);
                config.CreateMap<UserSignInDTO, ApplicationUser>(MemberList.Source);
                config.CreateMap<ApplicationUser, UserDTO>(MemberList.Destination);
                config.CreateMap<UserDTO, ApplicationUser>(MemberList.Source);
            });
        }
    }
}
