
using Hachodromo.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Swagger/OpenAPI services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddTransient<SeedDb>();

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
            app.MapControllers();
            app.UseCors(x=> x.AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(origin => true) // allow any origin
                            .AllowCredentials());   
            app.Run();
        }
    }
}
