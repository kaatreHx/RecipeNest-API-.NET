using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeNest.Models;
using RecipeNest.ServiceLayer;

namespace RecipeNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChefController : ControllerBase
    {

        private readonly IRecipeSL _recipeService;

        public ChefController(IRecipeSL recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost("chefLogin")]
        public async Task<ActionResult<string>> ChefLogin(string email, string password)
        {
            var (token, userId) = await _recipeService.ChefLoginAsync(email, password);
            if (string.IsNullOrEmpty(token))
            {
                // Return 401 Unauthorized with a custom message
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { Token = token, UserId = userId });
        }

        [HttpGet("chefs")]
        public async Task<ActionResult<IEnumerable<Chef>>> GetChef()
        {
            var recipes = await _recipeService.GetAllChefAsync();
            return Ok(recipes);
        }

        [HttpGet("chef/{chefId}")]
        public async Task<ActionResult<FoodLover>> GetChef(int chefId)
        {
            var user = await _recipeService.GetChefByIdAsync(chefId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("chef")]
        public async Task<ActionResult<Chef>> CreateChef([FromForm] ChefRequest chefRequest)
        {
            string profilePicPath = null;

            if (chefRequest.ProfilePic != null && chefRequest.ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(chefRequest.ProfilePic.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await chefRequest.ProfilePic.CopyToAsync(stream);
                }

                profilePicPath = $"/uploads/{uniqueFileName}";
            }

            var chef = new Chef
            {
                Id = chefRequest.Id,
                Name = chefRequest.Name,
                Email = chefRequest.Email,
                Password = chefRequest.Password,
                ProfilePic = profilePicPath, // Save the relative path
                JoinDate = DateTime.UtcNow
            };

            await _recipeService.AddChefAsync(chef);

            return CreatedAtAction(nameof(GetChef), new { id = chef.Id }, chef);
        }


        [HttpPut("chef/{chefId}")]
        public async Task<IActionResult> UpdateChef(int chefId, [FromForm] ChefRequest chefRequest)
        {
            if (chefId != chefRequest.Id)
                return BadRequest("ID in URL does not match ID in request body.");

            var existingChef = await _recipeService.GetChefByIdAsync(chefId);
            if (existingChef == null)
                return NotFound();

            // Update Name and Email
            existingChef.Name = chefRequest.Name;
            existingChef.Email = chefRequest.Email;

            // Handle profile picture upload
            if (chefRequest.ProfilePic != null && chefRequest.ProfilePic.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(chefRequest.ProfilePic.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await chefRequest.ProfilePic.CopyToAsync(stream);
                }

                var profilePicPath = $"/uploads/{uniqueFileName}";
                existingChef.ProfilePic = profilePicPath;
            }

            await _recipeService.UpdateChefAsync(existingChef);
            return NoContent();
        }


        [HttpPut("chef/{chefId}/change-password")]
        public async Task<IActionResult> ChangePassword(int chefId, [FromBody] ChangePasswordRequest request)
        {
            var result = await _recipeService.ChangeChefPasswordAsync(chefId, request.OldPassword, request.NewPassword);
            if (!result)
                return BadRequest("Old password is incorrect.");

            return Ok("Password changed successfully.");
        }



        [HttpDelete("chef/{chefId}")]
        public async Task<IActionResult> DeleteChef(int chefId)
        {
            var user = await _recipeService.GetChefByIdAsync(chefId);
            if (user == null)
            {
                return NotFound();
            }

            await _recipeService.DeleteChefAsync(chefId);
            return NoContent();
        }
    }
}
