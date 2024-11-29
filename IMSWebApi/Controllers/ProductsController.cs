using IMSWebApi.Data;
using IMSWebApi.DTO;
using IMSWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly InventoryDBaseContext _context;

        public ProductsController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
        //get products by supplierId


        [HttpGet("supplierId")]
        public async Task<IEnumerable<object>> GetProductBySupplierId(int id)
        {
            return await _context.Products.Where(product => product.SupplierId == id).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        //// PUT: api/Products/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProduct(int id, ProductDTO productdto)
        //{

        //    Product product = new Product();

        //    product.Name = productdto.Name;
        //    product.Description = productdto.Description;
        //    product.StockLevel = productdto.StockLevel;
        //    product.SupplierId = productdto.SupplierId;
        //    product.Price = productdto.Price;
        //    product.ReorderLevel = productdto.ReorderLevel;
        //    product.CategoryId = productdto.CategoryId;
        //    product.SKU = productdto.SKU;
        //    if (id != product.ProductId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(product).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productdto)
        {
            // Fetch the existing product from the database
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Update the properties of the existing product
            product.SKU = productdto.SKU;
            product.Name = productdto.Name;
            product.Description = productdto.Description;
            product.Price = productdto.Price;
            product.CategoryId = productdto.CategoryId;
            product.StockLevel = productdto.StockLevel;
            product.ReorderLevel = productdto.ReorderLevel;
            product.SupplierId = productdto.SupplierId;

            try
            {
                // Mark the entity as modified
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] ProductDTO productdto)
        {

            Product product = new Product();

            product.Name = productdto.Name;
            product.Description = productdto.Description;
            product.StockLevel = productdto.StockLevel;
            product.SupplierId = productdto.SupplierId;
            product.Price = productdto.Price;
            product.ReorderLevel = productdto.ReorderLevel;
            product.CategoryId = productdto.CategoryId;
            product.SKU = productdto.SKU;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
