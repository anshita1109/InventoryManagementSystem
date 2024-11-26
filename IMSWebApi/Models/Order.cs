namespace IMSWebApi.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // e.g., Pending, Completed, Returned

        public virtual Product Product { get; set; }
    }
}
