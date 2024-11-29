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
    public class OrdersController : ControllerBase
    {
        private readonly InventoryDBaseContext _context;

        public OrdersController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("GetByProductId/{productId}")]
        public IActionResult GetOrdersByProductId(int productId)
        {
            // Filter orders by ProductId and order them by OrderDate
            var orders = _context.Orders
                                 .Where(o => o.ProductId == productId) // Filter by ProductId
                                 .OrderBy(o => o.OrderDate)            // Optionally order by OrderDate
                                 .Select(o => new OrderDTO
                                 {
                                     OrderId = o.OrderId,
                                     ProductId = o.ProductId,
                                     Quantity = o.Quantity,
                                     Status = o.Status,
                                     OrderDate = o.OrderDate
                                 })
                                 .ToList();

            if (!orders.Any()) // Check if the list is empty
            {
                return NotFound($"No orders found for the specified ProductId: {productId}.");
            }

            return Ok(orders); // Return the filtered list of orders
        }


        // Other actions like GetAll, Post, Delete etc.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDTO orderdto)
        {
            // Fetch the existing order from the database
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update the properties of the existing order
            order.ProductId = orderdto.ProductId;
            order.Quantity = orderdto.Quantity;
            order.Status = orderdto.Status;
            order.OrderDate = orderdto.OrderDate;

            try
            {
                // Mark the entity as modified
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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



        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDTO orderdto)
        {
            Order order = new Order();

            order.ProductId = orderdto.ProductId;
            order.Quantity = orderdto.Quantity;
            order.Status = orderdto.Status;
            order.OrderDate = orderdto.OrderDate;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

    }
}