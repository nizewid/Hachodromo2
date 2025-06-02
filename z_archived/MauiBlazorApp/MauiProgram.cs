using Blazored.Modal;
using Hachodromo.MAUI.Auth;
using Hachodromo.Shared.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace MauiBlazorApp
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

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, MauiAuthenticationProvider>();
            builder.Services.AddScoped<ILoginService>(sp =>
                (MauiAuthenticationProvider)sp.GetRequiredService<AuthenticationStateProvider>());


 

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7062/") });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazoredModal();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
