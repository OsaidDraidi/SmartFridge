using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FridgeProject.Data.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? CreatorId { get; set; }

        [JsonIgnore]
        public ICollection<RecipeItem> RecipeItems { get; set; }    
    }
}
