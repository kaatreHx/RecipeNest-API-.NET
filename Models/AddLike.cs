namespace RecipeNest.Models
{
    public class AddLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
    }
}
