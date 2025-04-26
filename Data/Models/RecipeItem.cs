using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FridgeProject.Data.Models
{
    public class RecipeItem
    {
        [Key]
        public int Id { get; set; }
        public int Quantity {  get; set; }

        public int RecipeId {  get; set; }
        [JsonIgnore]
        public Recipe Recipe { get; set; }

        public int ItemId {  get; set; }
        public Item Item { get; set; }
    }
}
