using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Hachodromo.WEB.Auth;
using Hachodromo.WEB.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;

namespace Hachodromo.WEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7062/") });
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddSweetAlert2();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationProviderJWT>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
            builder.Services.AddBlazoredModal();
            builder.Services.AddMudServices();
            await builder.Build().RunAsync();
            CultureInfo culture = new("es-ES");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
