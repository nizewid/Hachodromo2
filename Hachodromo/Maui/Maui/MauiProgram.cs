using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Maui.Services;
using Maui.Shared.Auth;
using Maui.Shared.Repositories;
using Maui.Shared.Services;
using Maui.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Servicios Blazor y app
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<INativeDialogService, NativeDialogService>();
            // Servicios generales
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredModal();
            builder.Services.AddSweetAlert2();
            builder.Services.AddMudServices();
            builder.Services.AddBlazoredModal();

            // Autenticación
            builder.Services.AddScoped<AuthenticationProviderJWT>();
            builder.Services.AddScoped<AuthenticationStateProvider>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddScoped<ILoginService>(
                sp => sp.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddSingleton<ILogService, LogService>();

            // TokenStorage para MAUI

            builder.Services.AddScoped<ITokenStorage, SecureTokenStorage>();


            builder.Services.AddScoped(sp =>
            {
#if ANDROID
                var baseUrl = "http://192.168.0.29:5157/";
#else
    var baseUrl = "https://localhost:7062/";
#endif

                return new HttpClient { BaseAddress = new Uri(baseUrl) };
            });



            // Servicio de repositorio compartido
            builder.Services.AddScoped<IRepository, Repository>();

            return builder.Build();
        }
    }
}
