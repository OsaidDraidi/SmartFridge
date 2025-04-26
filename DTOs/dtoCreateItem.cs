using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeProject.DTOs
{
    public class dtoCreateItem
    {

        
        public string Name { get; set; }
        public int ExpiryDays { get; set; }
        public int CategoryId { get; set; }
       


    }
}
