namespace IMSWebApi.DTO
{
    public class ProductDTO
    {
        //public int ProductId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int StockLevel { get; set; }
        public int ReorderLevel { get; set; }
        public int? SupplierId { get; set; }
    }
}
