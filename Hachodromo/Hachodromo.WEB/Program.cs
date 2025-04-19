using CurrieTechnologies.Razor.SweetAlert2;
using Hachodromo.WEB.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Hachodromo.WEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7062/") });
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddSweetAlert2();

            await builder.Build().RunAsync();
        }
    }
}
