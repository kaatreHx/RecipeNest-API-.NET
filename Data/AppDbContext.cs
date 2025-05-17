using Microsoft.EntityFrameworkCore;
using RecipeNest.Models;

namespace RecipeNest.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<FoodLover> FoodLover { get; set; }
        public DbSet<Chef> Chef { get; set; }
        public DbSet<AddRecipe> AddRecipe { get; set; }
        public DbSet<AddLike> AddLike { get; set; }
        public DbSet<AddFavorite> AddFavorite { get; set; }
    }
}
