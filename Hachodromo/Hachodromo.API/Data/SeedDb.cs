using Hachodromo.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckRegionsAsync();
            await CheckCitiesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any()) // <-- cambiamos a "si NO hay países"
            {
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Countries ON");
                _context.Countries.AddRange(
                    new Country { Id = 1, Name = "España" }
                );

                await _context.SaveChangesAsync(); // 
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Countries OFF");
            }
        }


        private async Task CheckCitiesAsync()
        {
            if (!_context.Cities.Any())
            {
        //        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Cities ON");
                _context.Cities.AddRange(
                        // Andalucía
                        new City { CityCode = 4, CityName = "Almería", RegionId = 1 },
                        new City { CityCode = 11, CityName = "Cádiz", RegionId = 1 },
                        new City { CityCode = 14, CityName = "Córdoba", RegionId = 1 },
                        new City { CityCode = 18, CityName = "Granada", RegionId = 1 },
                        new City { CityCode = 21, CityName = "Huelva", RegionId = 1 },
                        new City { CityCode = 23, CityName = "Jaén", RegionId = 1 },
                        new City { CityCode = 29, CityName = "Málaga", RegionId = 1 },
                        new City { CityCode = 41, CityName = "Sevilla", RegionId = 1 },
                        // Aragón
                        new City { CityCode = 22, CityName = "Huesca", RegionId = 2 },
                        new City { CityCode = 44, CityName = "Teruel", RegionId = 2 },
                        new City { CityCode = 50, CityName = "Zaragoza", RegionId = 2 },
                        // Asturias, Principado de
                        new City { CityCode = 33, CityName = "Oviedo", RegionId = 3 },
                        new City { CityCode = 66, CityName = "Gijón", RegionId = 3 },
                        // Balears, Illes
                        new City { CityCode = 7, CityName = "Palma de Mallorca", RegionId = 4 },
                        // Canarias
                        new City { CityCode = 35, CityName = "Las Palmas de Gran Canaria", RegionId = 5 },
                        new City { CityCode = 38, CityName = "Santa Cruz de Tenerife", RegionId = 5 },
                        // Cantabria
                        new City { CityCode = 39, CityName = "Santander", RegionId = 6 },
                        // Castilla y León
                        new City { CityCode = 5, CityName = "Ávila", RegionId = 7 },
                        new City { CityCode = 9, CityName = "Burgos", RegionId = 7 },
                        new City { CityCode = 24, CityName = "León", RegionId = 7 },
                        new City { CityCode = 34, CityName = "Palencia", RegionId = 7 },
                        new City { CityCode = 37, CityName = "Salamanca", RegionId = 7 },
                        new City { CityCode = 40, CityName = "Segovia", RegionId = 7 },
                        new City { CityCode = 42, CityName = "Soria", RegionId = 7 },
                        new City { CityCode = 47, CityName = "Valladolid", RegionId = 7 },
                        new City { CityCode = 49, CityName = "Zamora", RegionId = 7 },
                        // Castilla-La Mancha
                        new City { CityCode = 2, CityName = "Albacete", RegionId = 8 },
                        new City { CityCode = 13, CityName = "Ciudad Real", RegionId = 8 },
                        new City { CityCode = 16, CityName = "Cuenca", RegionId = 8 },
                        new City { CityCode = 19, CityName = "Guadalajara", RegionId = 8 },
                        new City { CityCode = 45, CityName = "Toledo", RegionId = 8 },
                        // Cataluña
                        new City { CityCode = 8, CityName = "Barcelona", RegionId = 9 },
                        new City { CityCode = 17, CityName = "Girona", RegionId = 9 },
                        new City { CityCode = 25, CityName = "Lleida", RegionId = 9 },
                        new City { CityCode = 43, CityName = "Tarragona", RegionId = 9 },
                        // Comunitat Valenciana
                        new City { CityCode = 3, CityName = "Alicante/Alacant", RegionId = 10 },
                        new City { CityCode = 12, CityName = "Castellón/Castelló", RegionId = 10 },
                        new City { CityCode = 46, CityName = "Valencia/València", RegionId = 10 },
                        // Extremadura
                        new City { CityCode = 6, CityName = "Badajoz", RegionId = 11 },
                        new City { CityCode = 10, CityName = "Cáceres", RegionId = 11 },
                        // Galicia
                        new City { CityCode = 15, CityName = "A Coruña", RegionId = 12 },
                        new City { CityCode = 27, CityName = "Lugo", RegionId = 12 },
                        new City { CityCode = 32, CityName = "Ourense", RegionId = 12 },
                        new City { CityCode = 36, CityName = "Pontevedra", RegionId = 12 },
                        // Madrid, Comunidad de
                        new City { CityCode = 28, CityName = "Madrid", RegionId = 13 },
                        // Murcia, Región de
                        new City { CityCode = 30, CityName = "Murcia", RegionId = 14 },
                        // Navarra, Comunidad Foral de
                        new City { CityCode = 31, CityName = "Pamplona/Iruña", RegionId = 15 },
                        // País Vasco
                        new City { CityCode = 1, CityName = "Vitoria-Gasteiz", RegionId = 16 },
                        new City { CityCode = 48, CityName = "Bilbao", RegionId = 16 },
                        new City { CityCode = 20, CityName = "Donostia-San Sebastián", RegionId = 16 },
                        // Rioja, La
                        new City { CityCode = 26, CityName = "Logroño", RegionId = 17 },
                        // Ceuta
                        new City { CityCode = 51, CityName = "Ceuta", RegionId = 18 },
                        // Melilla
                        new City { CityCode = 52, CityName = "Melilla", RegionId = 19 }
                        );
            }
            await _context.SaveChangesAsync();
       //     await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Cities OFF");
        }

        private async Task CheckRegionsAsync()
        {
            if (!_context.Regions.Any())
            {
                //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Regions ON"); // no lo necesita 
                _context.Regions.AddRange(
                    new Region { RegionId = 1, RegionName = "Andalucía", CountryId = 1 },
                    new Region { RegionId = 2, RegionName = "Aragón", CountryId = 1 },
                    new Region { RegionId = 3, RegionName = "Asturias", CountryId = 1 },
                    new Region { RegionId = 4, RegionName = "Illes Balears", CountryId = 1 },
                    new Region { RegionId = 5, RegionName = "Canarias", CountryId = 1 },
                    new Region { RegionId = 6, RegionName = "Cantabria", CountryId = 1 },
                    new Region { RegionId = 7, RegionName = "Castilla y León", CountryId = 1 },
                    new Region { RegionId = 8, RegionName = "Castilla-La Mancha", CountryId = 1 },
                    new Region { RegionId = 9, RegionName = "Cataluña", CountryId = 1 },
                    new Region { RegionId = 10, RegionName = "Comunitat Valenciana", CountryId = 1 },
                    new Region { RegionId = 11, RegionName = "Extremadura", CountryId = 1 },
                    new Region { RegionId = 12, RegionName = "Galicia", CountryId = 1 },
                    new Region { RegionId = 13, RegionName = "Comunidad de Madrid", CountryId = 1 },
                    new Region { RegionId = 14, RegionName = "Región de Murcia", CountryId = 1 },
                    new Region { RegionId = 15, RegionName = "Comunidad Foral de Navarra", CountryId = 1 },
                    new Region { RegionId = 16, RegionName = "País Vasco", CountryId = 1 },
                    new Region { RegionId = 17, RegionName = "La Rioja", CountryId = 1 },
                    new Region { RegionId = 18, RegionName = "Ceuta", CountryId = 1 },
                    new Region { RegionId = 19, RegionName = "Melilla", CountryId = 1 }
                );
            }
            await _context.SaveChangesAsync();
            //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Regions OFF");
        }
    }
} 

