using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeNest.Models;
using RecipeNest.RepositoryLayer;
using RecipeNest.ServiceLayer;

namespace RecipeNest.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeNestController : ControllerBase
    {
        private readonly IRecipeSL _recipeService;

        public RecipeNestController(IRecipeSL recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("recipes")]
        public async Task<ActionResult<IEnumerable<AddRecipe>>> GetRecipe()
        {
            var recipes = await _recipeService.GetAllRecipeAsync();
            return Ok(recipes);
        }

        [HttpGet("recipes/{recipeId}")]
        public async Task<ActionResult<AddRecipe>> GetRecipe(int Id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(Id);
            if (recipe == null)
                return NotFound();
            return Ok(recipe);
        }

        [HttpPost("recipes")]
        public async Task<ActionResult<AddRecipe>> CreateRecipe([FromForm] AddRecipeRequest recipeRequest)
        {
            // Check if the chef exists
            bool chefExists = await _recipeService.ChefExistsAsync(recipeRequest.ChefId);
            if (!chefExists)
            {
                return BadRequest(new { message = "Chef does not exist" });
            }

            // Handle profile picture upload
            string profilePicPath = null;

            if (recipeRequest.ProfilePic != null && recipeRequest.ProfilePic.Length > 0)
            {
                // Define the folder where you want to store the uploaded images
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique filename to avoid overwriting existing files
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(recipeRequest.ProfilePic.FileName);

                // Get the full file path where the image will be saved
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the image file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await recipeRequest.ProfilePic.CopyToAsync(stream);
                }

                // Store the relative file path in the database
                profilePicPath = $"/uploads/{uniqueFileName}";  // Relative path to the file
            }

            // Create a new recipe object with the information
            var recipe = new AddRecipe
            {
                Id = recipeRequest.Id,
                Name = recipeRequest.Name,
                Category = recipeRequest.Category,
                Ingredients = recipeRequest.Ingredients,
                Time = recipeRequest.Time,
                ChefId = recipeRequest.ChefId,
                RecipeLike = recipeRequest.RecipeLike,
                ProfilePic = profilePicPath  // Store the relative path to the image
            };

            // Add the recipe to the database
            await _recipeService.AddRecipeAsync(recipe);

            // Return the newly created recipe
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }


        [HttpPut("recipes/{recipeId}")]
        public async Task<IActionResult> UpdateRecipe(int recipeId, [FromForm] AddRecipeRequest recipeRequest)
        {
            if (recipeId != recipeRequest.Id)
                return BadRequest(new { message = "Recipe ID mismatch." });

            // Fetch the existing recipe first
            var existingRecipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (existingRecipe == null)
                return NotFound(new { message = "Recipe not found." });

            string profilePicPath = existingRecipe.ProfilePic; // Default to existing image

            // If a new image is uploaded, replace it
            if (recipeRequest.ProfilePic != null && recipeRequest.ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(recipeRequest.ProfilePic.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await recipeRequest.ProfilePic.CopyToAsync(stream);
                }

                profilePicPath = $"/uploads/{uniqueFileName}";
            }

            // Update the recipe
            var recipe = new AddRecipe
            {
                Id = recipeRequest.Id,
                Name = recipeRequest.Name,
                Category = recipeRequest.Category,
                Ingredients = recipeRequest.Ingredients,
                Time = recipeRequest.Time,
                ChefId = recipeRequest.ChefId,
                ProfilePic = profilePicPath  // Important: keep existing if no new upload
            };

            await _recipeService.UpdateRecipeAsync(recipe);

            return NoContent();
        }



        [HttpDelete("recipes/{recipeId}")]
        public async Task<IActionResult> DeleteRecipe(int recipeId)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (recipe == null)
            {
                return NotFound();
            }

            await _recipeService.DeleteRecipeAsync(recipeId);
            return NoContent();
        }

        [HttpDelete("recipes/chef/{chefId}")]
        public async Task<IActionResult> DeleteRecipesByChef(int chefId)
        {
            var recipes = await _recipeService.GetRecipesByChefIdAsync(chefId);
            if (recipes == null || !recipes.Any())
            {
                return NotFound("No recipes found for the given chef.");
            }

            await _recipeService.DeleteRecipesByChefIdAsync(chefId);
            return NoContent();
        }


        [HttpPost("recipes/filter")]
        public async Task<ActionResult<IEnumerable<AddRecipe>>> FilterRecipes([FromBody] RecipeFilterRequest filterRequest)
        {
            var recipes = await _recipeService.FilterRecipesAsync(filterRequest);
            return Ok(recipes);
        }

        [HttpGet("recipes/trending")]
        public async Task<ActionResult<IEnumerable<AddRecipe>>> GetTrendingRecipes()
        {
            var recipes = await _recipeService.GetTrendingRecipesAsync();
            return Ok(recipes);
        }

        [HttpPost("like/{recipeId}/{userId}")]
        public async Task<IActionResult> LikeRecipeAsync(int recipeId, int userId)
        {
            try
            {
                await _recipeService.UpdateLikeAsync(recipeId, userId);
                return Ok("Recipe liked toggled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error liking recipe: {ex.Message}");
            }
        }

        [HttpPost("favorites/{recipeId}/{userId}")]
        public async Task<IActionResult> AddFavorite(int recipeId, int userId)
        {
            try
            {
                await _recipeService.AddFavoriteAsync(recipeId, userId);
                return Ok(new { message = "Recipe added to favorites successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("favorites/{favoriteId}")]
        public async Task<IActionResult> DeleteFavorite(int favoriteId)
        {
            try
            {
                await _recipeService.DeleteFavoriteAsync(favoriteId);
                return Ok(new { message = "Favorite removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("recipes/types")]
        public async Task<ActionResult<IEnumerable<string>>> GetRecipeTypes()
        {
            var recipeTypes = await _recipeService.GetRecipeTypesAsync();
            return Ok(recipeTypes);
        }

        [HttpGet("favorites/{userId}")]
        public async Task<IActionResult> GetFavorites(int userId)
        {
            try
            {
                var favoriteRecipes = await _recipeService.GetFavoritesByUserIdAsync(userId);

                if (favoriteRecipes == null || favoriteRecipes.Count == 0)
                {
                    return NotFound(new { message = "No favorite recipes found for this user." });
                }

                return Ok(favoriteRecipes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-like/{userId}/{recipeId}")]
        public async Task<IActionResult> CheckUserLike(int userId, int recipeId)
        {
            var hasLiked = await _recipeService.CheckUserLikedRecipeAsync(userId, recipeId);

            return Ok(new { liked = hasLiked });
        }

        [HttpGet("recipes/chef/{chefId}")]
        public async Task<ActionResult<IEnumerable<AddRecipe>>> GetRecipesByChef(int chefId)
        {
            var recipes = await _recipeService.GetRecipesByChefIdAsync(chefId);
            if (recipes == null || !recipes.Any())
            {
                return NotFound(new { message = "No recipes found for the given chef." });
            }

            return Ok(recipes);
        }


    }
}
