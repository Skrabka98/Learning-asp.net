using System.Data.Entity;

namespace MyFirstShopInASP.Models.Data
{
    public class Db : DbContext

    {
        public DbSet<PageDTO> Pages { get; set; }
    }
}