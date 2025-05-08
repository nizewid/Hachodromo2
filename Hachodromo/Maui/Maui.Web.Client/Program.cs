using Maui.Shared.Services;
using Maui.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Maui.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the Maui.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            await builder.Build().RunAsync();
        }
    }
}
