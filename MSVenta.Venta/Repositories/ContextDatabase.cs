using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Repositories
{
    public class ContextDatabase : DbContext
    {
        public ContextDatabase(DbContextOptions<ContextDatabase> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Models.Venta> Ventas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<ProductoAlmacen> ProductosAlmacenes { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar nombres de tablas
            modelBuilder.Entity<Cliente>().ToTable("cliente"); // 🚨 Nombre exacto de la tabla
            modelBuilder.Entity<Models.Venta>().ToTable("ventas");
            modelBuilder.Entity<Producto>().ToTable("producto");
            modelBuilder.Entity<Almacen>().ToTable("almacen");
            modelBuilder.Entity<ProductoAlmacen>().ToTable("producto_almacen");
            modelBuilder.Entity<DetalleVenta>().ToTable("detalle_venta");
            modelBuilder.Entity<Categoria>().ToTable("categoria");
            modelBuilder.Entity<Marca>().ToTable("marca");
            modelBuilder.Entity<Pago>().ToTable("pago");
            modelBuilder.Entity<MetodoPago>().ToTable("metodo_pago");
            // Configuraciones de relaciones

            modelBuilder.Entity<Models.Venta>(entity =>
            {
                // Mapear columnas
                entity.Property(v => v.ClienteId).HasColumnName("id_cliente"); // 🚨 Nombre real en DB
                entity.Property(v => v.UsuarioId).HasColumnName("id_usuario"); // Si existe en el modelo
                entity.Property(v => v.MontoTotal).HasColumnName("monto_total");
                // Relación con Cliente
                entity.HasOne(v => v.Cliente)
                      .WithMany()
                      .HasForeignKey(v => v.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductoAlmacen>(entity =>
            {
                entity.ToTable("producto_almacen");

                entity.Property(pa => pa.ProductoId)
                      .HasColumnName("id_producto");

                entity.Property(pa => pa.AlmacenId)
                      .HasColumnName("id_almacen");  // 👈 Asegúrate de que coincida con la DB

                entity.Property(pa => pa.Stock)
                      .HasColumnName("stock");

                // Relaciones
                entity.HasOne(pa => pa.Producto)
                      .WithMany()
                      .HasForeignKey(pa => pa.ProductoId);

                entity.HasOne(pa => pa.Almacen)
                      .WithMany()
                      .HasForeignKey(pa => pa.AlmacenId);
            });

            
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("detalle_venta");    

                // Mapear columnas
                entity.Property(dv => dv.ProductoAlmacenId)
                      .HasColumnName("id_producto_almacen"); // 🚨 Nombre real en la DB

                entity.Property(dv => dv.VentaId)
                      .HasColumnName("id_venta");

                // Configurar relaciones
                entity.HasOne(dv => dv.ProductoAlmacen)
                      .WithMany()
                      .HasForeignKey(dv => dv.ProductoAlmacenId);

                entity.HasOne(dv => dv.Venta)
                      .WithMany()
                      .HasForeignKey(dv => dv.VentaId);
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.StockMinimo).HasColumnName("stock_minimo");
                // Configurar la clave foránea
                entity.Property(p => p.CategoriaId)
                      .HasColumnName("id_categoria");
                // Configurar la clave foránea para Marca
                entity.Property(p => p.MarcaId)
                    .HasColumnName("id_marca");

            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.Property(p => p.FechaPago).HasColumnName("fecha_pago");
                entity.Property(p => p.MetodoPagoId).HasColumnName("id_metodo_pago");
                entity.Property(p => p.VentaId).HasColumnName("id_venta");
                // Relationships
                entity.HasOne(p => p.Venta)
                    .WithMany()
                    .HasForeignKey(p => p.VentaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.MetodoPago)
                    .WithMany()
                    .HasForeignKey(p => p.MetodoPagoId)
                    .IsRequired(false);
            });

        }
    }
}
