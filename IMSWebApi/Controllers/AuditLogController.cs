using IMSWebApi.Data;
using IMSWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly InventoryDBaseContext _context;

        public AuditLogController(InventoryDBaseContext context)
        {
            _context = context;
        }

        // GET: api/AuditLog
        [HttpGet]
        //public async Task<IActionResult> GetAuditLogs()
        //{
        //    // Retrieve all audit logs along with the related Supplier
        //    var auditLogs = await _context.AuditLogs.Include(a => a.Supplier).ToListAsync();

        //    if (auditLogs == null || !auditLogs.Any())
        //    {
        //        return NotFound("No audit logs found.");
        //    }

        //    return Ok(auditLogs);
        //}
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
        {
            var auditLogs = await _context.AuditLogs
                .Include(a => a.Supplier)  // Include the Supplier data
                .Select(auditLog => new
                {
                    auditLog.AuditLogId,
                    auditLog.Action,
                    auditLog.Description,
                    auditLog.ActionDate,
                    SupplierId = auditLog.Supplier.SupplierId,  // Select only SupplierId
                    SupplierName = auditLog.Supplier.Name      // Select only SupplierName
                })
                .ToListAsync();

            return Ok(auditLogs);
        }


        // POST: api/AuditLog/LogAction
        [HttpPost("LogAction")]
        public async Task<IActionResult> LogAction(int supplierId, string action, string description)
        {
            // Validate input
            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(description))
            {
                return BadRequest("Action and description are required.");
            }

            // Create a new audit log entry
            var auditLog = new AuditLog
            {
                SupplierID = supplierId,   // Associate with a specific Supplier
                Action = action,
                Description = description,
                ActionDate = DateTime.UtcNow   // Set the current time for ActionDate
            };

            // Add the new audit log to the context and save changes
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();

            return Ok("Action logged successfully.");
        }
    }
}