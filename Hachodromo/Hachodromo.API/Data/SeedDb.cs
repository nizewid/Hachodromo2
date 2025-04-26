using Hachodromo.API.Services;
using Hachodromo.Shared.Entities;
using Hachodromo.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;

        public SeedDb(DataContext context, IApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
        }
        public async Task CheckCountriesAsync()
        {
            var targetCountries = new List<(string Name, string Iso2)>
    {
        ("España", "ES"),
        ("Francia", "FR"),
        ("Portugal", "PT"),
        ("Andorra", "AD"),
        ("Gibraltar", "GI"),
        ("Marruecos", "MA"),
        ("Italia", "IT"),
        ("Reino Unido", "GB"),
        ("Alemania", "DE"),
        ("Países Bajos", "NL"),
        ("Estados Unidos", "US"),
        ("Bélgica", "BE"),
        ("Suecia", "SE"),
        ("Rusia", "RU"),
        ("Suiza", "CH"),
        ("Noruega", "NO"),
        ("Dinamarca", "DK"),
        ("Irlanda", "IE"),
        ("China", "CN"),
        ("Japón", "JP"),
        ("Canadá", "CA"),
        ("Australia", "AU"),
        ("Venezuela", "VE")
    };

            foreach (var (name, iso2) in targetCountries)
            {
                if (_context.Countries.Any(c => c.Name == name))
                    continue;

                Country country = new() { Name = name, Regions = new List<Region>() };

                Response responseRegions = await _apiService.GetListAsync<RegionResponse>("/v1", $"/countries/{iso2}/states");

                if (responseRegions.IsSuccess)
                {
                    List<RegionResponse> regionResponses = (List<RegionResponse>)responseRegions.Result!;
                    foreach (RegionResponse regionResponse in regionResponses)
                    {
                        Region region = new()
                        {
                            RegionName = regionResponse.Name!,
                            Cities = new List<City>()
                        };

                        Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{iso2}/states/{regionResponse.Iso2}/cities");

                        if (responseCities.IsSuccess)
                        {
                            List<CityResponse> cityResponses = (List<CityResponse>)responseCities.Result!;
                            var cityNames = new HashSet<string>();

                            foreach (CityResponse cityResponse in cityResponses)
                            {
                                if (string.IsNullOrWhiteSpace(cityResponse.Name))
                                    continue;

                                var cityName = cityResponse.Name!.Trim();

                                // Evitar duplicados agregando (ciudad) si ya existe
                                if (!cityNames.Add(cityName))
                                {
                                    cityName += " (ciudad)";
                                }

                                region.Cities!.Add(new City { CityName = cityName });
                            }
                        }

                        if (region.Cities!.Count > 0)
                        {
                            country.Regions!.Add(region);
                        }
                    }
                }

                if (country.Regions!.Count > 0)
                {
                    _context.Countries!.Add(country);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}


