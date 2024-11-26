namespace IMSWebApi.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string Name { get; set; } = null!;

        public string ContactInfo { get; set; } = null!;

        public string Address { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
    }
}
