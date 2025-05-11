using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Maui.Shared.Auth;
using Maui.Shared.Repositories;
using Maui.Shared.Services;
using Maui.Web.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Maui.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the Maui.Shared project

            // HttpClient para llamar a la API
            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri("https://localhost:7062/") });

            // Registro de librerías de UI externas (solo se usan en Web)
            builder.Services.AddSweetAlert2();         // Para popups con SweetAlert2
            builder.Services.AddMudServices();         // Servicios de MudBlazor
            builder.Services.AddBlazoredModal();       // Servicio para modales Blazored
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Auth
            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<AuthenticationProviderJWT>();
            builder.Services.AddScoped<AuthenticationStateProvider>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddScoped<ILoginService>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddScoped<ITokenStorage, LocalStorageTokenStorage>();
            // Ejemplo de servicio común
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddScoped<INativeDialogService, NativeDialogService>();

            await builder.Build().RunAsync();
        }
    }
}
