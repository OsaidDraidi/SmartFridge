using FridgeProject.Data.Models;

namespace FridgeProject.DTOs
{
    public class dtoGetRecipe
    {

        
        
        public int RecipeId { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }

        public List<dtoGetRecipeItem>? items { get; set; }
    }
}
