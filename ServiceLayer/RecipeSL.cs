using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RecipeNest.DTO;
using RecipeNest.Models;
using RecipeNest.RepositoryLayer;

namespace RecipeNest.ServiceLayer
{
    public class RecipeSL : IRecipeSL
    {
        private readonly IRecipeRL _recipeRL;
        private readonly PasswordHasher<FoodLover> _passwordHasher;
        private readonly PasswordHasher<Chef> _passwordChefHasher;

        public RecipeSL(IRecipeRL recipeRL)
        {
            _recipeRL = recipeRL;
            _passwordHasher = new PasswordHasher<FoodLover>();
            _passwordChefHasher = new PasswordHasher<Chef>();
        }
        public async Task AddFoodLoverAsync(FoodLover foodlover)
        {
            foodlover.Password = _passwordHasher.HashPassword(foodlover, foodlover.Password);
            await _recipeRL.AddFoodLoverAsync(foodlover);
        }

        public async Task DeleteFoodLoverAsync(int id)
        {
            await _recipeRL.DeleteFoodLoverAsync(id);
        }

        public async Task<IEnumerable<FoodLover>> GetAllFoodLoverAsync()
        {
            return await _recipeRL.GetAllFoodLoverAsync();
        }

        public async Task<FoodLover> GetFoodLoverByEmailAsync(string email)
        {
            return await _recipeRL.GetFoodLoverByEmailAsync(email);
        }

        public async Task<FoodLover> GetFoodLoverByIdAsync(int id)
        {
            return await _recipeRL.GetFoodLoverByIdAsync(id);
        }

        public async Task UpdateFoodLoverAsync(FoodLover foodlover)
        {
            await _recipeRL.UpdateFoodLoverAsync(foodlover);
        }

        public async Task<(string? Token, int? UserId)> LoginAsync(string email, string password)
        {
            var user = await _recipeRL.GetFoodLoverByEmailAsync(email);
            if (user == null) return (null, null); 

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJwtToken(user); 
                return (token, user.Id);
            }

            return (null, null); 
        }



        private string GenerateJwtToken(FoodLover user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_very_long_256_bit_secret_key_here_that_is_at_least_32_characters_long"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "RecipeNest",
                audience: "RecipeNestAPI",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IEnumerable<Chef>> GetAllChefAsync()
        {
            return await _recipeRL.GetAllChefAsync();
        }

        public async Task<Chef> GetChefByIdAsync(int id)
        {
            return await _recipeRL.GetChefByIdAsync(id);
        }

        public async Task<Chef> GetChefByEmailAsync(string email)
        {
            return await _recipeRL.GetChefByEmailAsync(email);
        }

        public async Task<(string? Token, int? ChefId)> ChefLoginAsync(string email, string password)
        {
            var chef = await _recipeRL.GetChefByEmailAsync(email);
            if (chef == null) return (null, null); 

            var result = _passwordChefHasher.VerifyHashedPassword(chef, chef.Password, password);
            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateChefJwtToken(chef); 
                return (token, chef.Id);
            }

            return (null, null); 
        }


        private string GenerateChefJwtToken(Chef user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_very_long_256_bit_secret_key_here_that_is_at_least_32_characters_long"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "RecipeNest",
                audience: "RecipeNestAPI",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task AddChefAsync(Chef chef)
        {
            chef.Password = _passwordChefHasher.HashPassword(chef, chef.Password);
            await _recipeRL.AddChefAsync(chef);
        }

        public async Task UpdateChefAsync(Chef chef)
        {
            await _recipeRL.UpdateChefAsync(chef);
        }

        public async Task<bool> ChangeChefPasswordAsync(int chefId, string oldPassword, string newPassword)
        {
            var chef = await _recipeRL.GetChefByIdAsync(chefId);
            if (chef == null)
                return false; // Or throw custom exception "Chef not found"

            var verificationResult = _passwordChefHasher.VerifyHashedPassword(chef, chef.Password, oldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                return false;

            // Old password correct, now hash the new password
            chef.Password = _passwordChefHasher.HashPassword(chef, newPassword);
            await _recipeRL.UpdateChefAsync(chef);

            return true;
        }

        public async Task<bool> ChangeFoodLoverPasswordAsync(int id, string oldPassword, string newPassword)
        {
            var user = await _recipeRL.GetFoodLoverByIdAsync(id);
            if (user == null)
                return false; // Or throw custom exception "Customer not found"

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, oldPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
                return false;

            // Old password correct, now hash the new password
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _recipeRL.UpdateFoodLoverAsync(user);

            return true;
        }

        public async Task DeleteChefAsync(int id)
        {
            await _recipeRL.DeleteChefAsync(id);
        }

        public async Task<IEnumerable<AddRecipe>> GetAllRecipeAsync()
        {
            return await _recipeRL.GetAllRecipeAsync();
        }

        public async Task<AddRecipe> GetRecipeByIdAsync(int id)
        {
            return await _recipeRL.GetRecipeByIdAsync(id);
        }

        public async Task<bool> ChefExistsAsync(int chefId)
        {
            var chef = await _recipeRL.GetChefByIdAsync(chefId);
            return chef != null;
        }

        public async Task AddRecipeAsync(AddRecipe recipe)
        {
            await _recipeRL.AddRecipeAsync(recipe);
        }

        public async Task UpdateRecipeAsync(AddRecipe recipe)
        {
            var existingRecipe = await _recipeRL.GetRecipeByIdAsync(recipe.Id);

            if (existingRecipe == null)
            {
                throw new Exception("Recipe not found");
            }

            // Update fields except ChefId
            existingRecipe.Name = recipe.Name;
            existingRecipe.Ingredients = recipe.Ingredients;
            existingRecipe.Time = recipe.Time;
            existingRecipe.ProfilePic = recipe.ProfilePic;

            await _recipeRL.UpdateRecipeAsync(existingRecipe);
        }


        public async Task DeleteRecipeAsync(int id)
        {
            await _recipeRL.DeleteRecipeAsync(id);
        }

        public async Task UpdateLikeAsync(int recipeId, int userId)
        {
            await _recipeRL.AddLikeAsync(recipeId, userId);
        }

        public async Task AddFavoriteAsync(int recipeId, int userId)
        {
            await _recipeRL.AddFavoriteAsync(recipeId, userId);
        }

        public async Task DeleteFavoriteAsync(int favoriteId)
        {
            await _recipeRL.DeleteFavoriteAsync(favoriteId);
        }

        public async Task<IEnumerable<AddRecipe>> FilterRecipesAsync(RecipeFilterRequest filterRequest)
        {
            return await _recipeRL.FilterRecipesAsync(filterRequest);
        }

        public async Task<IEnumerable<AddRecipe>> GetTrendingRecipesAsync()
        {
            return await _recipeRL.GetTrendingRecipesAsync();
        }

        public async Task<IEnumerable<string>> GetRecipeTypesAsync()
        {
            return await _recipeRL.GetRecipeTypesAsync();
        }

        public async Task<IEnumerable<AddRecipe>> GetRecipesByChefIdAsync(int chefId)
        {
            return await _recipeRL.GetRecipesByChefIdAsync(chefId);
        }

        public async Task DeleteRecipesByChefIdAsync(int chefId)
        {
            var recipes = await GetRecipesByChefIdAsync(chefId);
            if (recipes.Any())
            {
                await _recipeRL.DeleteRecipesByChefIdAsync(chefId);
            }
        }

        public async Task<List<FavoriteRecipeDto>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _recipeRL.GetFavoritesByUserIdAsync(userId);
        }

        public async Task<bool> CheckUserLikedRecipeAsync(int userId, int recipeId)
        {
            return await _recipeRL.UserLikedRecipeAsync(userId, recipeId);
        }
        public async Task<IEnumerable<AddRecipe>> RecipesByChefIdAsync(int chefId)
        {
            return await _recipeRL.GetRecipesByChefIdAsync(chefId);
        }
    }
}
