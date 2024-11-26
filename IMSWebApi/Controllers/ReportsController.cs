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
        private readonly InventoryDBaseContext _context;

        // Injecting the DbContext via constructor for dependency injection
        public ReportsController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET api/reports/inventoryreport
        [HttpGet("inventoryreport")]
        public async Task<ActionResult> GetReport()
        {
            try
            {
                // LINQ query to get SupplierId, Supplier Name, ProductId, StockLevel, ReorderLevel, and Quantity from related tables
                var reportData = await (from supplier in _context.Suppliers
                                        join product in _context.Products on supplier.SupplierId equals product.SupplierId
                                        join order in _context.Orders on product.ProductId equals order.ProductId
                                        select new
                                        {
                                            supplier.SupplierId,
                                            SupplierName = supplier.Name,
                                            product.ProductId,
                                            StockLevel = product.StockLevel,
                                            ReorderLevel = product.ReorderLevel,
                                            Quantity = order.Quantity
                                        }).ToListAsync();

                // If no data is returned, return a specific message
                if (reportData == null || reportData.Count == 0)
                {
                    return NotFound("No report data available.");
                }

                // Return the result as JSON
                return Ok(reportData);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                // In a production environment, you could use a logging framework like Serilog or NLog
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //[HttpGet("supplier-performance")]
        //public async Task<ActionResult> GetSupplierPerformanceReport()
        //{
        //    try
        //    {
        //        // LINQ query to fetch SupplierId, SupplierName, and the total number of products supplied
        //        var supplierPerformance = await _context.Suppliers
        //            .Select(s => new
        //            {
        //                s.SupplierId,                  // Supplier ID
        //                s.Name,                         // Supplier Name
        //                TotalProductsSupplied = s.Products.Count() // Count the products supplied by the supplier
        //            })
        //            .ToListAsync();

        //        // Check if no data is found
        //        if (supplierPerformance == null || !supplierPerformance.Any())
        //        {
        //            return NotFound("No supplier performance data available.");
        //        }

        //        // Return the result as JSON
        //        return Ok(supplierPerformance);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions and return a 500 Internal Server Error
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        [HttpGet("supplier-performance")]
        public async Task<ActionResult> GetSupplierPerformanceReport()
        {
            try
            {
                // Step 1: Get total products supplied by each supplier
                var supplierPerformance = await _context.Suppliers
                    .Select(s => new
                    {
                        s.SupplierId,                  // Supplier ID
                        s.Name,                         // Supplier Name
                        TotalProductsSupplied = s.Products.Count() // Count the products supplied by the supplier
                    })
                    .ToListAsync();

                // Step 2: Find the supplier with the maximum products supplied
                var maxSupplier = supplierPerformance.OrderByDescending(sp => sp.TotalProductsSupplied)
                                                      .FirstOrDefault();

                // Step 3: Add a flag to indicate which supplier has supplied the most products
                var result = supplierPerformance.Select(s => new
                {
                    s.SupplierId,
                    s.Name,
                    s.TotalProductsSupplied,
                    IsTopSupplier = s.SupplierId == maxSupplier?.SupplierId // Flag to indicate top supplier
                }).ToList();

                // Check if no data is found
                if (result == null || !result.Any())
                {
                    return NotFound("No supplier performance data available.");
                }

                // Return the result as JSON
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a 500 Internal Server Error
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
