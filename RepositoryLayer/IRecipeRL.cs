using RecipeNest.DTO;
using RecipeNest.Models;
using RecipeNest.ServiceLayer;

namespace RecipeNest.RepositoryLayer
{
    public interface IRecipeRL
    {
        //FoodLover
        Task<IEnumerable<FoodLover>> GetAllFoodLoverAsync();
        Task<FoodLover> GetFoodLoverByIdAsync(int id);
        Task<FoodLover> GetFoodLoverByEmailAsync(string email);
        Task AddFoodLoverAsync(FoodLover foodlover);
        Task UpdateFoodLoverAsync(FoodLover foodlover);
        Task DeleteFoodLoverAsync(int id);

        //Chef
        Task<IEnumerable<Chef>> GetAllChefAsync();
        Task<Chef> GetChefByIdAsync(int id);
        Task<Chef> GetChefByEmailAsync(string email);
        Task AddChefAsync(Chef chef);
        Task UpdateChefAsync(Chef chef);
        Task DeleteChefAsync(int id);

        //Recipe
        Task<IEnumerable<AddRecipe>> GetAllRecipeAsync();
        Task<AddRecipe> GetRecipeByIdAsync(int id);
        Task<bool> ChefExistsAsync(int chefId);
        Task AddRecipeAsync(AddRecipe recipe);
        Task UpdateRecipeAsync(AddRecipe recipe);
        Task DeleteRecipeAsync(int id);
        Task<IEnumerable<AddRecipe>> FilterRecipesAsync(RecipeFilterRequest filterRequest);
        Task<IEnumerable<AddRecipe>> GetTrendingRecipesAsync();
        Task<IEnumerable<string>> GetRecipeTypesAsync();
        Task<IEnumerable<AddRecipe>> GetRecipesByChefIdAsync(int chefId);
        Task DeleteRecipesByChefIdAsync(int chefId);
        Task<IEnumerable<AddRecipe>> RecipesByChefIdAsync(int chefId);


        //Like
        Task AddLikeAsync(int recipeId, int userId);
        Task<bool> UserLikedRecipeAsync(int userId, int recipeId);
        //Favorite
        Task AddFavoriteAsync(int recipeId, int userId);
        Task DeleteFavoriteAsync(int favoriteId);
        Task<List<FavoriteRecipeDto>> GetFavoritesByUserIdAsync(int userId);

    }
}
