using System.ComponentModel.DataAnnotations;

namespace MovieFlix.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Action { get; set; }           
        public string EntityName { get; set; }       
        public int? EntityId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }         
    }

}
