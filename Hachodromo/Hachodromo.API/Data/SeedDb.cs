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
        private readonly IFileStorage _fileStorage;
        private string[] categories = { "Ropa", "Accesorios", "Vikingos" };
        private string[] itemImages = { "camisaviking.jpg", "camisaviking2.jpg", "cuernoVikingo.jpg", "llaveroviking.jpg" };
        private string[] sitesPhotos = { "A.jpg", "B.jpeg", "C.jpg" };
        private string[] profileImages = { "Anais.png", "coco.jpg", "profile.jpg", "maya.png" };


        public SeedDb(DataContext context, IApiService apiService, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _context = context;
            _apiService = apiService;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public async Task SeedAsync()
        {
            //await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();

            await CheckRolesAsync();
            await CheckCategoriesAsync();
            await CheckItemsAsync();
            await CheckMembershipsAsync();
            await CheckCountriesAsync();
            await CheckSiteAsync("Hachodromo OVD","Hachodromo de Oviedo","Calle Progreso 36","600111222","Oviedo", sitesPhotos[0], (6, TargetStatus.Available),(4, TargetStatus.Available));
            await CheckSiteAsync("Hachodromo Gij","Hachodromo de Gijón","Calle Alegría 12","900900900","Gijón",sitesPhotos[1],(5, TargetStatus.Available),(6, TargetStatus.UnderMaintenance));
            await CheckUserAsync("13364217K", "José", "Flores Silva", "jgfs.jf@gmail.com", "640097444", profileImages[2], "C/Progreso 36", UserType.Admin, 1);
            await CheckUserAsync("11223344A", "Anais", "Gonzalez", "anais@example.com", "600111222", profileImages[0], "Calle Luna 5", UserType.User, 2);
            await CheckUserAsync("11222556B", "Coco", "Flores", "crutraucreibroimu-2614@yopmail.com", "600333444", profileImages[1], "Calle Sol 9", UserType.User, 3);
            await CheckUserAsync("44112233B", "Maya", "Gonzalez", "maya@yopmail.com", "600333444", profileImages[3], "Calle Sol 9", UserType.User, 4);
        }

        private async Task CheckSiteAsync(string name,string description,string address,string phone,string cityName, string photoSite, params (int capacity, TargetStatus status)[] targetsData)
        {

            if (await _context.Sites.AnyAsync(s => s.Name == name))
                return;

            var city = await _context.Cities
                .FirstOrDefaultAsync(c => c.CityName == cityName);
            if (city == null)
                throw new Exception($"La ciudad '{cityName}' no existe en la base de datos.");

            var filePath = $"{Environment.CurrentDirectory}\\Images\\sites\\{photoSite}";
            var fileBytes = File.ReadAllBytes(filePath);
            var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "sites");
            var site = new Site
            {
                Name = name,
                Description = description,
                Address = address,
                Phone = phone,
                Photo = imagePath,
                City = city,
                CityId = city.CityId,
                Targets = targetsData
                    .Select(t => new Target { Capacity = t.capacity, Status = t.status })
                    .ToList()
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();
        }


        private async Task CheckMembershipsAsync()
        {
            if (!_context.Memberships.Any())
            {
                _context.Memberships.Add(new Membership
                {
                    Name = "Sin Membresía",
                    Description = "Sin Membresía",
                    Price = 0.00M,
                    Duration = 0,
                    Discount = 0.00M
                });

                _context.Memberships.Add(new Membership
                {
                    Name = "Vikingo Aficionado",
                    Description = "Descuento en dianas",
                    Price = 10.00M,
                    Duration = 6,
                    Discount = 0.05M
                });

                _context.Memberships.Add(new Membership
                {
                    Name = "Maestro de Valkirias",
                    Description = "Descuento en dianas y artículos",
                    Price = 20.00M,
                    Duration = 12,
                    Discount = 0.10M
                });

                _context.Memberships.Add(new Membership
                {
                    Name = "Dios de Valhala",
                    Description = "Descuento en dianas, artículos y eventos",
                    Price = 35.00M,
                    Duration = 12,
                    Discount = 0.15M
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckItemsAsync()
        {
            if (!_context.Items.Any())
            {
                await AddItemAsync("Camiseta Vikinga", 17.55M, 6F, new List<string>() { categories[0], categories[2] }, new List<string>() { itemImages[0], itemImages[1] });
                await AddItemAsync("Cuerno Vikingo", 50.00M, 6F, new List<string>() { categories[1], categories[2] }, new List<string>() { itemImages[2] });
                await AddItemAsync("Llavero Vikingo", 5.00M, 3F, new List<string>() { categories[1], categories[2] }, new List<string>() { itemImages[3] });
                await AddItemAsync("Camiseta Vikinga Edición 2", 18.99M, 6F, new List<string>() { categories[0], categories[2] }, new List<string>() { itemImages[1] });
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddItemAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Item item = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ItemCategories = new List<ItemCategory>(),
                ItemImages = new List<ItemImage>()
            };

            foreach (var categoryName in categories)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category != null)
                {
                    item.ItemCategories.Add(new ItemCategory { Category = category });
                }
            }

            foreach (string? image in images)
            {
                var filePath = $"{Environment.CurrentDirectory}\\Images\\items\\{image}";
                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "items");
                item.ItemImages.Add(new ItemImage { Image = imagePath });
            }

            _context.Items.Add(item);
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = categories[0] });
                _context.Categories.Add(new Category { Name = categories[1] });
                _context.Categories.Add(new Category { Name = categories[2] });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string profilePhoto, string address, UserType userType, int membershipId)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                var city = _context.Cities.FirstOrDefault(x => x.CityName == "Gijón");
                if (city == null)
                {
                    city = await _context.Cities.FirstOrDefaultAsync();
                }

                var membership = await _context.Memberships.FirstOrDefaultAsync(m => m.Id == membershipId);
                if (membership == null)
                {
                    membership = await _context.Memberships.FirstOrDefaultAsync();
                }

                var filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{profilePhoto}";
                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");
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
                    MembershipId = membershipId,
                    Membership = membership,
                    UserType = userType,
                    Photo = imagePath,
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
        ("España", "ES"),/*,
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
        ("Australia", "AU"),*/
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


