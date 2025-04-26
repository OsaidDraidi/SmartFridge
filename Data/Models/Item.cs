using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeProject.Data.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int ExpiryDays { get; set; }
        public string? CreatorId {  get; set; }
        
        

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }


        public ICollection<UserItem> UserItems { get; set; }
        public ICollection<RecipeItem> RecipeItems { get; set; }
    }
}
