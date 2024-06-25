using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Homework.Application.Commands.Users;
using Homework.Application.Identity;
using Homework.Application.Queries.Users;
using Homework.Application.Services;
using Homework.Common;
using Homework.Domain.Models.Identity;
using Homework.Persistence.Data;
using Homework.Persistence.ServiceImplementations;
using Serilog;
using System.Globalization;
using System.Text;

namespace Homework.Web.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;
            
            // Add Entity Framework db contexts
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(config.GetConnectionString(Constants.DB_CONN_IDENTITY_NAME)));

            // Add Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()

                .CreateLogger();

            builder.Host.UseSerilog();

            // Add Identity configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1440);
                opt.Lockout.MaxFailedAccessAttempts = 7;
            })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders();

            //Add CORS Policy 
            builder.Services.AddCors(x =>
            {
                x.AddPolicy("DevelopmentCors",
                    b =>
                    {
                        b.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            // Add services to the container.
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            // Registrations
            builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(LoginQuery).Assembly));
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAdministrationService, AdministrationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Fluent Validations registrations
            builder.Services.AddScoped<IValidator<LoginQuery>, LoginQueryValidator>();
            builder.Services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();

            // Localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("sr")
                };

                options.DefaultRequestCulture = new RequestCulture("sr");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });           

            // Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddControllers()
                .AddOData(option => option.Select().Filter().Count().OrderBy().Expand());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Homework.Web.Api", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("DevelopmentCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}