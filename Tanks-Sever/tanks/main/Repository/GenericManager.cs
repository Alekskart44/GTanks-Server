using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.loaders;
using Tanks_Sever.tanks.users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Repository
{
    public static class GenericManager
    {
        private static GameDbContext DbContext { get; set; }

        public static IGenericRepository repository;

        public static void Init(string config)
        {
            var loader = new DBConfiguratorLoader(config).GetConnectionString();
            var options = new DbContextOptionsBuilder<GameDbContext>().UseMySql(loader, ServerVersion.AutoDetect(loader)).Options;
            DbContext = new GameDbContext(options);
            DbContext.Database.Migrate();
            repository = new GenericRepository(DbContext);
        }

    }
}
