namespace FridgeProject.DTOs
{
    public class dtoRecipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<dtoCreateRecipeItem> items { get; set; }
    }
}
