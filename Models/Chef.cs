namespace RecipeNest.Models
{
    public class ChefRequest
    {
        public int Id { get; set; }                     // Chef Id
        public string Name { get; set; }                // Chef's name
        public string Email { get; set; }               // Chef's email
        public string? Password { get; set; }            // Chef's password 
        public IFormFile? ProfilePic { get; set; }         // Chef's profile picture
        public DateTime? JoinDate { get; set; }          // Chef's join date
    }
    public class Chef
    {
        public int Id { get; set; }                     // Chef Id
        public string Name { get; set; }                // Chef's name
        public string Email { get; set; }               // Chef's email
        public string? Password { get; set; }            // Chef's password 
        public string? ProfilePic { get; set; }         // Chef's profile picture
        public DateTime? JoinDate { get; set; }          // Chef's join date
    }
}
