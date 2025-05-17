using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeNest.Models
{
    public class FoodLoverRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public IFormFile? ProfilePic { get; set; } // <-- Uploaded Image
        public DateTime? JoinDate { get; set; }
    }
    public class FoodLover
    {
        public int Id { get; set; }                 // FoodLover ID
        public string Name { get; set; }            // FoodLover Name
        public string Email { get; set; }           // FoodLover Email
        public string? Password { get; set; }        // FoodLover Password
        public string? ProfilePic { get; set; } // FoodLover Profile Picture     
        public DateTime? JoinDate { get; set; }      // FoodLover Join Date  
    }
}
