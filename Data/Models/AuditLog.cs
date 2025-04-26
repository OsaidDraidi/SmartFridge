using System.ComponentModel.DataAnnotations;

namespace FridgeProject.Data.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } // المستخدم الذي قام بالتغيير
        public string Action { get; set; } // نوع العملية (إضافة، تعديل، حذف)
        public string EntityName { get; set; } // اسم الجدول اللي بدي اعدل عليه بالداتا بيس 
        public int EntityId { get; set; } // رقم العنصر اللي عدلت عليه او اضفته
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // وقت العملية
    }

}
