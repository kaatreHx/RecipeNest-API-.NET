namespace RecipeNest.DTO
{
    public class FavoriteRecipeDto
    {
        public int FavoriteId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string ProfilePic { get; set; }
        public int ChefId { get; set; }
        public int RecipeLike { get; set; }
    }
}
