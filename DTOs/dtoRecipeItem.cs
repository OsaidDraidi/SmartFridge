using FridgeProject.Data.Models;

namespace FridgeProject.DTOs
{
    public class dtoRecipeItem
    {
        public int Id { get; set; }
        public int RequirdQuantity { get; set; }
        public int RecipeId { get; set; }
        public string? RecipeName { get; set; }

        public int ItemId { get; set; }

        public string? ItemName { get; set; }
    }
}
