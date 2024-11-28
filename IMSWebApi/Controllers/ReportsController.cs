
using IMSWebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        //    private readonly InventoryDBaseContext _context;


        private readonly InventoryDBaseContext _context;

        // Injecting the DbContext via constructor for dependency injection
        public ReportsController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET api/reports/inventoryreport
        [HttpGet("stocklevelreport")]
        public async Task<ActionResult> GetStockLevelReport()
        {
            try
            {
                // Step 1: Perform a left join between Suppliers and Products
                var supplierProducts = await (from supplier in _context.Suppliers
                                              join product in _context.Products
                                              on supplier.SupplierId equals product.SupplierId into supplierProductGroup
                                              from product in supplierProductGroup.DefaultIfEmpty()
                                              select new
                                              {
                                                  SupplierId = supplier.SupplierId,
                                                  SupplierName = supplier.Name,
                                                  ProductId = product != null ? product.ProductId : 0, // Use 0 for missing ProductId
                                                  ProductName = product != null ? product.Name : "N/A", // Use "N/A" for missing ProductName
                                                  StockLevel = product != null ? product.StockLevel : 0, // Use 0 for missing StockLevel
                                                  ReorderLevel = product != null ? product.ReorderLevel : 0 // Use 0 for missing ReorderLevel
                                              }).ToListAsync();

                // Step 2: Fetch order quantities grouped by ProductId
                var orderQuantities = await _context.Orders
                    .GroupBy(o => o.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(o => o.Quantity)
                    })
                    .ToListAsync();

                // Step 3: Merge supplier-products data with order quantities
                var stockLevelReport = supplierProducts.GroupJoin(
                    orderQuantities,
                    sp => sp.ProductId,
                    oq => oq.ProductId,
                    (sp, oqGroup) => new
                    {
                        sp.SupplierId,
                        sp.SupplierName,
                        sp.ProductId,
                        sp.ProductName,
                        sp.StockLevel,
                        sp.ReorderLevel,
                        Quantity = oqGroup.Any() ? oqGroup.Sum(oq => oq.Quantity) : 0 // Use 0 if no orders
                    }).ToList();

                // Step 4: Return the report or a "NotFound" message if empty
                if (!stockLevelReport.Any())
                {
                    return NotFound("No inventory report data available.");
                }

                return Ok(stockLevelReport);
            }
            catch (Exception ex)
            {
                // Log and return error details
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("supplier-summary")]
        public IActionResult GetSupplierSummary()
        {
            var supplierReport = _context.Orders
                .Where(o => o.Status == "Completed") // Only include completed orders
                .Join(_context.Products, // Join Orders with Products
                    order => order.ProductId,
                    product => product.ProductId,
                    (order, product) => new { order, product }) // Anonymous type with order and product

                .Join(_context.Suppliers, // Join Products with Suppliers
                    op => op.product.SupplierId,
                    supplier => supplier.SupplierId,
                    (op, supplier) => new { op.order, op.product, supplier }) // Anonymous type with order, product, and supplier

                .GroupBy(o => o.supplier.SupplierId) // Group by SupplierId
                .Select(g => new
                {
                    SupplierId = g.Key,
                    SupplierName = g.Select(o => o.supplier.Name).FirstOrDefault(),
                    TotalQuantity = g.Sum(o => o.order.Quantity), // Total quantity of completed orders
                    TotalRevenue = g.Sum(o => o.product.Price * o.order.Quantity) // Total revenue from completed orders
                })
                .ToList();

            return Ok(supplierReport);
        }
    }
}
