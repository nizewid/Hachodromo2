using Hachodromo.API.Helpers;
using Hachodromo.API.Services;
using Hachodromo.Shared.Entities;
using Hachodromo.Shared.Enums;
using Hachodromo.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IApiService apiService, IUserHelper userHelper)
        {
            _context = context;
            _apiService = apiService;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("13364217K","José","Flores Silva","jgfs.jf@gmail.com","640097444","C/Progreso 36",UserType.Admin);
        }
        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                var city = _context.Cities.FirstOrDefault(x => x.CityName == "Gijón");
                if (city == null)
                {
                    city = await _context.Cities.FirstOrDefaultAsync();
                }
                user = new User
                {
                    Document = document,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phone,
                    BornDate = new DateTime(1989, 9, 19),
                    Address = address,
                    UserName = email,
                    City = city,
                    UserType = userType,
                };
                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRole(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }
            return user;
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


