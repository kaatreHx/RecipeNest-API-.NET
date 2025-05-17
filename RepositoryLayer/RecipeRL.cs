using Microsoft.EntityFrameworkCore;
using RecipeNest.Data;
using RecipeNest.DTO;
using RecipeNest.Models;
using RecipeNest.ServiceLayer;

namespace RecipeNest.RepositoryLayer
{
    public class RecipeRL : IRecipeRL
    {
        private readonly AppDbContext _context;


        public RecipeRL(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddChefAsync(Chef chef)
        {
            await _context.Chef.AddAsync(chef);
            await _context.SaveChangesAsync();
        }

        public async Task AddFoodLoverAsync(FoodLover foodlover)
        {
            await _context.FoodLover.AddAsync(foodlover);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChefExistsAsync(int chefId)
        {
            return await _context.Chef.AnyAsync(c => c.Id == chefId);
        }

        public async Task AddRecipeAsync(AddRecipe recipe)
        {
            await _context.AddRecipe.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChefAsync(int id)
        {
            var chef = await _context.Chef.FindAsync(id);
            if (chef != null)
            {
                _context.Chef.Remove(chef);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteFoodLoverAsync(int id)
        {
            var foodLover = await _context.FoodLover.FindAsync(id);
            if (foodLover != null)
            {
                _context.FoodLover.Remove(foodLover);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _context.AddRecipe.FindAsync(id);
            if (recipe != null)
            {
                _context.AddRecipe.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Chef>> GetAllChefAsync()
        {
            return await _context.Chef.ToListAsync();
        }

        public async Task<IEnumerable<FoodLover>> GetAllFoodLoverAsync()
        {
            return await _context.FoodLover.ToListAsync();
        }

        public async Task<IEnumerable<AddRecipe>> GetAllRecipeAsync()
        {
            return await _context.AddRecipe.ToListAsync();
        }

        public async Task<Chef> GetChefByEmailAsync(string email)
        {
            return await _context.Chef.FirstOrDefaultAsync(f => f.Email == email);
        }

        public async Task<Chef> GetChefByIdAsync(int id)
        {
            return await _context.Chef.FindAsync(id);
        }

        public async Task<FoodLover> GetFoodLoverByEmailAsync(string email)
        {
            return await _context.FoodLover.FirstOrDefaultAsync(f => f.Email == email);
        }

        public async Task<FoodLover> GetFoodLoverByIdAsync(int id)
        {
            return await _context.FoodLover.FindAsync(id);
        }

        public async Task<AddRecipe> GetRecipeByIdAsync(int id)
        {
            return await _context.AddRecipe.FindAsync(id);
        }

        public async Task UpdateChefAsync(Chef chef)
        {
            _context.Chef.Update(chef);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFoodLoverAsync(FoodLover foodlover)
        {
            _context.FoodLover.Update(foodlover);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipeAsync(AddRecipe recipe)
        {
            _context.AddRecipe.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task AddLikeAsync(int recipeId, int userId)
        {
            var recipe = await _context.AddRecipe.FindAsync(recipeId);

            if (recipe == null)
            {
                throw new Exception("Recipe not found");
            }

            var existingLike = await _context.AddLike
            .FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.AddLike.Remove(existingLike);
                recipe.RecipeLike -= 1; 
            }
            else
            {
                var like = new AddLike
                {
                    RecipeId = recipeId,
                    UserId = userId
                };

                await _context.AddLike.AddAsync(like);
                recipe.RecipeLike += 1; 
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddFavoriteAsync(int recipeId, int userId)
        {
            var recipe = await _context.AddRecipe.FindAsync(recipeId);

            if (recipe == null)
            {
                throw new Exception("Recipe not found");
            }

            var existingFavorite = await _context.AddFavorite
            .FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.UserId == userId);

            if (existingFavorite != null)
            {
                throw new Exception("Recipe is already in your favorites");
            }

            var favorite = new AddFavorite
            {
                RecipeId = recipeId,
                UserId = userId
            };

            await _context.AddFavorite.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFavoriteAsync(int favoriteId)
        {
            var favorite = await _context.AddFavorite.FindAsync(favoriteId);
            if (favorite != null)
            {
                _context.AddFavorite.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AddRecipe>> FilterRecipesAsync(RecipeFilterRequest filterRequest)
        {
            var query = _context.AddRecipe.AsQueryable();

            if (!string.IsNullOrEmpty(filterRequest.Name))
            {
                query = query.Where(r => r.Name.Contains(filterRequest.Name));
            }

            if (!string.IsNullOrEmpty(filterRequest.Category))
            {
                query = query.Where(r => r.Category == filterRequest.Category);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<AddRecipe>> GetTrendingRecipesAsync()
        {
            return await _context.AddRecipe
                .OrderByDescending(r => r.RecipeLike)
                .Take(10) 
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRecipeTypesAsync()
        {
            
            var recipeTypes = await _context.AddRecipe
                .Select(r => r.Category) 
                .Distinct()
                .ToListAsync();

            return recipeTypes;
        }

        public async Task<IEnumerable<AddRecipe>> GetRecipesByChefIdAsync(int chefId)
        {
            return await _context.AddRecipe
                .Where(r => r.ChefId == chefId)  // Filter by chefId
                .ToListAsync();  // Return all recipes for that chef
        }

        public async Task DeleteRecipesByChefIdAsync(int chefId)
        {
            var recipes = await _context.AddRecipe
                .Where(r => r.ChefId == chefId)
                .ToListAsync();

            if (recipes.Any())
            {
                _context.AddRecipe.RemoveRange(recipes);  // Remove all recipes for the given chef
                await _context.SaveChangesAsync();  // Save the changes
            }
        }

        public async Task<List<FavoriteRecipeDto>> GetFavoritesByUserIdAsync(int userId)
        {
            var favorites = await _context.AddFavorite
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                .ToListAsync();

            var favoriteRecipes = favorites.Select(fav => new FavoriteRecipeDto
            {
                FavoriteId = fav.Id,                
                RecipeId = fav.Recipe.Id,            
                Name = fav.Recipe.Name,
                Ingredients = fav.Recipe.Ingredients,
                ProfilePic = fav.Recipe.ProfilePic,
                ChefId = fav.Recipe.ChefId,
                RecipeLike = fav.Recipe.RecipeLike
            }).ToList();

            return favoriteRecipes;
        }

        public async Task<bool> UserLikedRecipeAsync(int userId, int recipeId)
        {
            return await _context.AddLike
                .AnyAsync(like => like.UserId == userId && like.RecipeId == recipeId);
        }

        public async Task<IEnumerable<AddRecipe>> RecipesByChefIdAsync(int chefId)
        {
            return await _context.AddRecipe
                                 .Where(r => r.ChefId == chefId)
                                 .ToListAsync();
        }

    }
}
