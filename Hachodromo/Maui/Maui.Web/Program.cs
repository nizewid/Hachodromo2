// Usos de librerías de UI y autenticación compartida
using Blazored.Modal;                         // Modal dialogs para Blazor
using CurrieTechnologies.Razor.SweetAlert2;   // SweetAlert2 para popups bonitos
using Maui.Shared.Auth;                       // Proveedor de autenticación personalizado JWT
using Maui.Shared.Repositories;               // Repositorio compartido (acceso a datos)
using Maui.Shared.Services;                   // Servicios generales compartidos (si los hay)
using Maui.Web.Components;                    // Componentes propios del host (si tienes)
using Maui.Web.Services;                      // Servicios propios del host (si tienes)
using Microsoft.AspNetCore.Components.Authorization; // Para manejar el estado de autenticación
using MudBlazor.Services;                     // Servicios del framework de UI MudBlazor

namespace Maui.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Crea el WebApplicationBuilder para construir una app ASP.NET Core
            var builder = WebApplication.CreateBuilder(args);

            // Configura soporte para Razor Components (Blazor Server + WebAssembly híbrido)
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents(); // Habilita componentes interactivos de WASM

            // Servicio de ejemplo para identificar la plataforma (mobile/desktop/web)
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // HttpClient que se inyectará a los servicios/clientes para llamar a la API
            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri("https://localhost:7062/") });

            // Registro de librerías de UI externas (solo se usan en Web)
            builder.Services.AddSweetAlert2();         // Para popups con SweetAlert2
            builder.Services.AddMudServices();         // Servicios de MudBlazor
            builder.Services.AddBlazoredModal();       // Servicio para modales Blazored

            // Habilita el sistema de autorización de Blazor
            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<ILogService, LogService>();

            // Registro del proveedor de autenticación personalizado basado en JWT
            builder.Services.AddScoped<AuthenticationProviderJWT>();

            // Blazor espera un AuthenticationStateProvider para saber si el usuario está autenticado
            builder.Services.AddScoped<AuthenticationStateProvider>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());

            // Servicio para iniciar sesión y cerrar sesión desde componentes
            builder.Services.AddScoped<ILoginService>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());

            // Registro de tu servicio de datos (puede usar HttpClient internamente)
            builder.Services.AddScoped<IRepository, Repository>();

            builder.Services.AddScoped<ITokenStorage, NullTokenStorage>();
            
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // ⚠️ Este Build sobra — está fuera de lugar y luego vuelve a hacerse más abajo
            //await builder.Build().RunAsync(); // ❌ Elimínalo. El correcto es el siguiente Build

            // 🔧 Construcción final de la aplicación
            var app = builder.Build();

            // Configura el entorno de desarrollo o producción
            if (app.Environment.IsDevelopment())
            {
                // Habilita herramientas de depuración para WebAssembly
                app.UseWebAssemblyDebugging();
            }
            else
            {
                // En producción, captura errores globales y aplica políticas de seguridad
                app.UseExceptionHandler("/Error");
                app.UseHsts(); // Aplica HSTS (seguridad HTTPS en navegadores)
            }

            // Redirige automáticamente HTTP a HTTPS
            app.UseHttpsRedirection();

            // 🧱 Sirve archivos estáticos (CSS, JS, imágenes, etc.)
            app.UseStaticFiles();

            // 🔐 Requerido para componentes interactivos protegidos por antifalsificación (formulario seguro)
            app.UseAntiforgery();

            // ⬇️ Mapea el componente raíz (App.razor) y carga Blazor WebAssembly en modo interactivo
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode() // Usa Blazor WASM + Server para interactividad inicial
                .AddAdditionalAssemblies(              // Ensamblados adicionales donde buscar componentes/rutas
                    typeof(Maui.Shared._Imports).Assembly,       // UI compartida
                    typeof(Maui.Web.Client._Imports).Assembly);  // Cliente WASM

            // 🔁 Ruta de fallback: cualquier URL no encontrada redirige al index.html (SPA)
            app.MapFallbackToFile("index.html");

            // 🚀 Inicia la aplicación web
            app.Run();
        }
    }
}
