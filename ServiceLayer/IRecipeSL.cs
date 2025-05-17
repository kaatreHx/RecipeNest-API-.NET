using RecipeNest.DTO;
using RecipeNest.Models;
using RecipeNest.RepositoryLayer;

namespace RecipeNest.ServiceLayer
{
    public interface IRecipeSL
    {
        //FoodLover
        Task<IEnumerable<FoodLover>> GetAllFoodLoverAsync();
        Task<FoodLover> GetFoodLoverByIdAsync(int id);
        Task<FoodLover> GetFoodLoverByEmailAsync(string email);
        Task<(string? Token, int? UserId)> LoginAsync(string email, string password);
        Task AddFoodLoverAsync(FoodLover foodlover);
        Task UpdateFoodLoverAsync(FoodLover foodlover);
        Task DeleteFoodLoverAsync(int id);
        Task<bool> ChangeFoodLoverPasswordAsync(int id, string oldPassword, string newPassword);

        //Chef
        Task<IEnumerable<Chef>> GetAllChefAsync();
        Task<Chef> GetChefByIdAsync(int id);
        Task<Chef> GetChefByEmailAsync(string email);
        Task<(string? Token, int? ChefId)> ChefLoginAsync(string email, string password);
        Task AddChefAsync(Chef chef);
        Task UpdateChefAsync(Chef chef);
        Task DeleteChefAsync(int id);
        Task<bool> ChangeChefPasswordAsync(int chefId, string oldPassword, string newPassword);


        //Recipe
        Task<IEnumerable<AddRecipe>> GetAllRecipeAsync();
        Task<AddRecipe> GetRecipeByIdAsync(int id);
        Task<bool> ChefExistsAsync(int chefId);
        Task AddRecipeAsync(AddRecipe recipe);
        Task UpdateRecipeAsync(AddRecipe recipe);
        Task DeleteRecipeAsync(int id);
        Task<IEnumerable<AddRecipe>> GetTrendingRecipesAsync();
        Task<IEnumerable<string>> GetRecipeTypesAsync();
        Task<IEnumerable<AddRecipe>> GetRecipesByChefIdAsync(int chefId);
        Task DeleteRecipesByChefIdAsync(int chefId);
        Task<IEnumerable<AddRecipe>> RecipesByChefIdAsync(int chefId);


        //Like
        Task UpdateLikeAsync(int recipeId, int userId);
        Task<bool> CheckUserLikedRecipeAsync(int userId, int recipeId);
        //Favorite
        Task AddFavoriteAsync(int recipeId, int userId);
        Task DeleteFavoriteAsync(int favoriteId);
        Task<IEnumerable<AddRecipe>> FilterRecipesAsync(RecipeFilterRequest filterRequest);
        Task<List<FavoriteRecipeDto>> GetFavoritesByUserIdAsync(int userId);
    }
}
