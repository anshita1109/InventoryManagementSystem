using IMSWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IMSWebApi.Data
{
    public class InventoryDBaseContext : DbContext
    {
        public InventoryDBaseContext(DbContextOptions<InventoryDBaseContext> options): base(options) { }


        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

    }
}
