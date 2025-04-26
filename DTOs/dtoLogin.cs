using System.ComponentModel.DataAnnotations;

namespace FridgeProject.DTOs
{
    public class dtoLogin
    {
        [Required]
        public string Email {  get; set; }
        [Required]
        public string Password { get; set; }
    }
}
