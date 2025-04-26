using System.ComponentModel.DataAnnotations;

namespace FridgeProject.DTOs
{
    public class dtoRegister
    {
        [Required]
        public string Username {  get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
