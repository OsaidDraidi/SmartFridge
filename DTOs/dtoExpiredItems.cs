namespace FridgeProject.DTOs
{
    public class dtoExpiredItems
    {
        public int Id { get; set; }
        public string ItemName { get; set; }    
        public string CategoryName { get; set; }
        public DateOnly CreateDate { get; set; }
        public int Quantity { get; set; }
        
        
    }
}
