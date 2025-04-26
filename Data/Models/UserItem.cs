using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeProject.Data.Models
{
    public class UserItem
    {
        [Key]
        public int Id { get; set; }
        public DateOnly CreateDate { get; set; }
        public int Quantity {  get; set; }
        
        public string CreatorId {  get; set; }
        public string? CreatorEmail { get; set; }


        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public Item Item { get; set; }



        //[ForeignKey(nameof(User))]
        //public int UserId { get; set; } 
        //public User User { get; set; }
    }
}
