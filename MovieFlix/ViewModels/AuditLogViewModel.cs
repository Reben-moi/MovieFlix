namespace MovieFlix.ViewModels
{
    public class AuditLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }  
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string Details { get; set; }
    }

}


