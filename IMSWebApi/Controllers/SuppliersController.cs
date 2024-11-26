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
    public class SuppliersController : ControllerBase
    {
        private readonly InventoryDBaseContext _context;

        public SuppliersController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierDTO supplierdto)
        {
            // Fetch the existing supplier from the database
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            // Update the properties of the supplier
            supplier.Name = supplierdto.Name;
            supplier.ContactInfo = supplierdto.ContactInfo;
            supplier.Address = supplierdto.Address;

            try
            {
                // Mark the entity as modified and save changes
                _context.Entry(supplier).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            // Create an audit log entry for the update action
            var auditLog = new AuditLog
            {
                Action = "Update Supplier",
                Description = $"Updated supplier with ID {supplier.SupplierId}.",
                SupplierID = supplier.SupplierId,
                ActionDate = DateTime.Now
            };
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(SupplierDTO supplierdto)
        {
            Supplier supplier = new Supplier();

            supplier.Name = supplierdto.Name;
            supplier.ContactInfo = supplierdto.ContactInfo;
            supplier.Address = supplierdto.Address;
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            await _context.AuditLogs.AddAsync(new AuditLog
            {
                Action = "Add Supplier",
                Description = " Adding Supplier",
                SupplierID = supplier.SupplierId,
                ActionDate = DateTime.Now,
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
