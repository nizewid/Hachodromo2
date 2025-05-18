using Hachodromo.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hachodromo.API.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            var connectionString = "Data Source=JOSE_FLORES\\SQLEXPRESS;Initial Catalog=Hachodrom;Persist Security Info=True;User ID=joseadmin;Password=18817795;Trust Server Certificate=True";
            optionsBuilder.UseSqlServer(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}
