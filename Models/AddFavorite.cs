namespace RecipeNest.Models
{
    public class AddFavorite
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }

        public FoodLover User { get; set; }
        public AddRecipe Recipe { get; set; }
    }
}
