using System.Text.Json.Serialization;

namespace RecipeNest.Models
{

    public class AddRecipeRequest
    {
        public int Id { get; set; }
        public IFormFile? ProfilePic { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Ingredients { get; set; }
        public string Time { get; set; }
        public int ChefId { get; set; }
        public int RecipeLike { get; set; }
    }

    public class AddRecipe
    {
        public int Id { get; set; }
        public string? ProfilePic { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Ingredients { get; set; }
        public string Time { get; set; }
        public int ChefId { get; set; }
        public int RecipeLike { get; set; }
    }
}
