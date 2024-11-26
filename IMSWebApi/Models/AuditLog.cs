namespace IMSWebApi.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int SupplierID { get; set; }
        public string Action {  get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; } 
        public virtual Supplier Supplier { get; set; }


    }
}
