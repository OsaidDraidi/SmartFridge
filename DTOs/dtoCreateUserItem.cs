using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeProject.DTOs
{
    public class dtoCreateUserItem
    {
        public int Id { get; set; }
        public DateOnly CreateDate { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        

    }
}
