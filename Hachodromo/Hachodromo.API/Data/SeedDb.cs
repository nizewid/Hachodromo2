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
            await SeedSpainAsync();
        }

        public async Task SeedSpainAsync()
        {
            // Si ya hay países, asumimos que la BD está cargada
            if (await _context.Countries.AnyAsync())
                return;

            // --- País  → Regiones → Ciudades (sin CityCode) ------------------------
            var spain = new Country
            {
                Name = "España",
                Regions = new List<Region>
        {
            new Region
            {
                RegionName = "Andalucía",
                Cities = new List<City>
                {
                    new City { CityName = "Almería"  },
                    new City { CityName = "Cádiz"    },
                    new City { CityName = "Córdoba"  },
                    new City { CityName = "Granada"  },
                    new City { CityName = "Huelva"   },
                    new City { CityName = "Jaén"     },
                    new City { CityName = "Málaga"   },
                    new City { CityName = "Sevilla"  }
                }
            },
            new Region
            {
                RegionName = "Aragón",
                Cities = new List<City>
                {
                    new City { CityName = "Huesca"    },
                    new City { CityName = "Teruel"    },
                    new City { CityName = "Zaragoza"  }
                }
            },
            new Region
            {
                RegionName = "Asturias",
                Cities = new List<City>
                {
                    new City { CityName = "Oviedo" },
                    new City { CityName = "Gijón"  }
                }
            },
            new Region
            {
                RegionName = "Illes Balears",
                Cities = new List<City>
                {
                    new City { CityName = "Palma de Mallorca" }
                }
            },
            new Region
            {
                RegionName = "Canarias",
                Cities = new List<City>
                {
                    new City { CityName = "Las Palmas de Gran Canaria" },
                    new City { CityName = "Santa Cruz de Tenerife"     }
                }
            },
            new Region
            {
                RegionName = "Cantabria",
                Cities = new List<City>
                {
                    new City { CityName = "Santander" }
                }
            },
            new Region
            {
                RegionName = "Castilla y León",
                Cities = new List<City>
                {
                    new City { CityName = "Ávila"      },
                    new City { CityName = "Burgos"     },
                    new City { CityName = "León"       },
                    new City { CityName = "Palencia"   },
                    new City { CityName = "Salamanca"  },
                    new City { CityName = "Segovia"    },
                    new City { CityName = "Soria"      },
                    new City { CityName = "Valladolid" },
                    new City { CityName = "Zamora"     }
                }
            },
            new Region
            {
                RegionName = "Castilla-La Mancha",
                Cities = new List<City>
                {
                    new City { CityName = "Albacete"     },
                    new City { CityName = "Ciudad Real"  },
                    new City { CityName = "Cuenca"       },
                    new City { CityName = "Guadalajara"  },
                    new City { CityName = "Toledo"       }
                }
            },
            new Region
            {
                RegionName = "Cataluña",
                Cities = new List<City>
                {
                    new City { CityName = "Barcelona" },
                    new City { CityName = "Girona"    },
                    new City { CityName = "Lleida"    },
                    new City { CityName = "Tarragona" }
                }
            },
            new Region
            {
                RegionName = "Comunitat Valenciana",
                Cities = new List<City>
                {
                    new City { CityName = "Alicante/Alacant" },
                    new City { CityName = "Castellón/Castelló" },
                    new City { CityName = "Valencia/València" }
                }
            },
            new Region
            {
                RegionName = "Extremadura",
                Cities = new List<City>
                {
                    new City { CityName = "Badajoz" },
                    new City { CityName = "Cáceres" }
                }
            },
            new Region
            {
                RegionName = "Galicia",
                Cities = new List<City>
                {
                    new City { CityName = "A Coruña"   },
                    new City { CityName = "Lugo"       },
                    new City { CityName = "Ourense"    },
                    new City { CityName = "Pontevedra" }
                }
            },
            new Region
            {
                RegionName = "Comunidad de Madrid",
                Cities = new List<City>
                {
                    new City { CityName = "Madrid" }
                }
            },
            new Region
            {
                RegionName = "Región de Murcia",
                Cities = new List<City>
                {
                    new City { CityName = "Murcia" }
                }
            },
            new Region
            {
                RegionName = "Comunidad Foral de Navarra",
                Cities = new List<City>
                {
                    new City { CityName = "Pamplona/Iruña" }
                }
            },
            new Region
            {
                RegionName = "País Vasco",
                Cities = new List<City>
                {
                    new City { CityName = "Vitoria-Gasteiz"          },
                    new City { CityName = "Bilbao"                   },
                    new City { CityName = "Donostia-San Sebastián"   }
                }
            },
            new Region
            {
                RegionName = "La Rioja",
                Cities = new List<City>
                {
                    new City { CityName = "Logroño" }
                }
            },
            new Region
            {
                RegionName = "Ceuta",
                Cities = new List<City>
                {
                    new City { CityName = "Ceuta" }
                }
            },
            new Region
            {
                RegionName = "Melilla",
                Cities = new List<City>
                {
                    new City { CityName = "Melilla" }
                }
            }
        }
            };

            _context.Countries.Add(spain);
            await _context.SaveChangesAsync();
        }

    }
} 

