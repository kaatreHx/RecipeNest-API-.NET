using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeNest.Models;
using RecipeNest.ServiceLayer;

namespace RecipeNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly IRecipeSL _recipeService;

        public CustomerController(IRecipeSL recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost("foodLoverLogin")]
        public async Task<ActionResult<string>> Login(string email, string password)
        {
            var (token, userId) = await _recipeService.LoginAsync(email, password);
            if (string.IsNullOrEmpty(token))
            {
                // Return 401 Unauthorized with a custom message
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { Token = token, UserId = userId });
        }

        [HttpGet("foodlovers")]
        public async Task<ActionResult<IEnumerable<FoodLover>>> GetFoodLover()
        {
            var recipes = await _recipeService.GetAllFoodLoverAsync();
            return Ok(recipes);
        }

        [HttpGet("foodlover/{id}")]
        public async Task<ActionResult<FoodLover>> GetFoodLover(int id)
        {
            var user = await _recipeService.GetFoodLoverByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("foodlover")]
        public async Task<ActionResult<FoodLover>> CreateFoodLover([FromForm] FoodLoverRequest foodLoverRequest)
        {
            string profilePicPath = null;

            if (foodLoverRequest.ProfilePic != null && foodLoverRequest.ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(foodLoverRequest.ProfilePic.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foodLoverRequest.ProfilePic.CopyToAsync(stream);
                }

                profilePicPath = $"/uploads/{uniqueFileName}";
            }

            var user = new FoodLover
            {
                Id = foodLoverRequest.Id,
                Name = foodLoverRequest.Name,
                Email = foodLoverRequest.Email,
                Password = foodLoverRequest.Password,
                ProfilePic = profilePicPath,  // Now it's a URL path string
                JoinDate = DateTime.UtcNow
            };

            await _recipeService.AddFoodLoverAsync(user);

            return CreatedAtAction(nameof(GetFoodLover), new { id = user.Id }, user);
        }


        [HttpPut("foodlover/{id}")]
        public async Task<IActionResult> UpdateFoodLover(int id, [FromForm] FoodLoverRequest foodLoverRequest)
        {
            if (id != foodLoverRequest.Id)
                return BadRequest("ID in URL does not match ID in request body.");

            var existingFoodLover = await _recipeService.GetFoodLoverByIdAsync(id);
            if (existingFoodLover == null)
                return NotFound();

            // Update Name and Email only
            existingFoodLover.Name = foodLoverRequest.Name;
            existingFoodLover.Email = foodLoverRequest.Email;

            // Handle profile picture update
            if (foodLoverRequest.ProfilePic != null && foodLoverRequest.ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(foodLoverRequest.ProfilePic.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foodLoverRequest.ProfilePic.CopyToAsync(stream);
                }

                var profilePicPath = $"/uploads/{uniqueFileName}";
                existingFoodLover.ProfilePic = profilePicPath;
            }

            await _recipeService.UpdateFoodLoverAsync(existingFoodLover);

            return NoContent();
        }


        [HttpPut("foodlover/{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var result = await _recipeService.ChangeFoodLoverPasswordAsync(id, request.OldPassword, request.NewPassword);
            if (!result)
                return BadRequest("Old password is incorrect.");

            return Ok("Password changed successfully.");
        }


        [HttpDelete("foodlover/{id}")]
        public async Task<IActionResult> DeleteFoodLover(int id)
        {
            var foodLover = await _recipeService.GetFoodLoverByIdAsync(id);
            if (foodLover == null)
            {
                return NotFound();
            }

            await _recipeService.DeleteFoodLoverAsync(id);
            return NoContent();
        }
    }
}
