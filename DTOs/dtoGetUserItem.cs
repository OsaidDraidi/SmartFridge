namespace FridgeProject.DTOs
{
    public class dtoGetUserItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public DateOnly CreateDate { get; set; }
        public int Quantity { get; set; }
        public string? CreatorEmail { get; set; }
    }
}
