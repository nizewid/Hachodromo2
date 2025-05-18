using Hachodromo.API.Data;
using Hachodromo.API.Helpers;
using Hachodromo.API.Services;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Hachodromo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Swagger/OpenAPI services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
          Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
          Example: 'Bearer 12345abcdef'<br /> <br />",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
            });

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IApiService, ApiService>();
            builder.Services.AddScoped<IUserHelper, UserHelper>();
            builder.Services.AddScoped<IFileStorage, FileStorage>();
            builder.Services.AddScoped<IMailHelper, MailHelper>();
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(x =>
            {
                x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                x.SignIn.RequireConfirmedEmail = true;
                x.User.RequireUniqueEmail = true;

                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 6;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;

                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // TODO: change to 5 min
                x.Lockout.MaxFailedAccessAttempts = 3;
                x.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<DataContext>()  // 👈 asegúrate que DataContext ya usa Guid también
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero,
                });

            var app = builder.Build();
            SeedData(app);

            void SeedData(WebApplication app)
            {
                IServiceScopeFactory? scopeFactory = app.Services.GetService<IServiceScopeFactory>();
                using (IServiceScope? scope = scopeFactory?.CreateScope())
                {
                    SeedDb? service = scope?.ServiceProvider.GetService<SeedDb>();
                    service!.SeedAsync().Wait();
                }
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Genera el JSON
                app.UseSwaggerUI(); // Muestra la UI en /swagger
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();
            app.UseCors(x => x.AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(origin => true) // allow any origin
                            .AllowCredentials());
            app.Run();
        }
    }
}
