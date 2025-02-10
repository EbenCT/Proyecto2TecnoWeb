using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Repositories
{
    public class ContextDatabase : DbContext
    {
        public ContextDatabase(DbContextOptions<ContextDatabase> options) : base(options) { }


        public DbSet<Producto> Productos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<ProductoAlmacen> ProductosAlmacenes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<AjusteInventario> AjustesInventario { get; set; }
        public DbSet<DetalleAjuste> DetallesAjuste { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar nombres de tablas

            modelBuilder.Entity<Producto>().ToTable("producto");
            modelBuilder.Entity<Almacen>().ToTable("almacen");
            modelBuilder.Entity<ProductoAlmacen>().ToTable("producto_almacen");
            modelBuilder.Entity<Categoria>().ToTable("categoria");
            modelBuilder.Entity<Marca>().ToTable("marca");
            modelBuilder.Entity<AjusteInventario>().ToTable("ajuste_inventario");
            modelBuilder.Entity<DetalleAjuste>().ToTable("detalle_ajuste");
            // Configuraciones de relaciones

           
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");

                // Configurar la clave foránea
                entity.Property(p => p.categoriaId)
                      .HasColumnName("id_categoria");
                // Configurar la clave foránea para Marca
                entity.Property(p => p.marcaId)
                    .HasColumnName("id_marca");

            });
            modelBuilder.Entity<ProductoAlmacen>()
                .HasOne(pa => pa.Producto)
                .WithMany()
                .HasForeignKey(pa => pa.ProductoId);

            modelBuilder.Entity<ProductoAlmacen>(entity =>
            {
                entity.ToTable("producto_almacen");

                entity.Property(pa => pa.ProductoId)
                      .HasColumnName("id_producto");

                entity.Property(pa => pa.AlmacenId)
                      .HasColumnName("id_almacen");  // 👈 Asegúrate de que coincida con la DB

                entity.Property(pa => pa.stock)
                      .HasColumnName("stock");

                // Relaciones
                entity.HasOne(pa => pa.Producto)
                      .WithMany()
                      .HasForeignKey(pa => pa.ProductoId);

                entity.HasOne(pa => pa.Almacen)
                      .WithMany()
                      .HasForeignKey(pa => pa.AlmacenId);
            });
                        
            modelBuilder.Entity<AjusteInventario>(entity =>
            {
                entity.Property(e => e.usuarioId).HasColumnName("usuario_id");
            });

            modelBuilder.Entity<DetalleAjuste>(entity =>
            {
                entity.ToTable("detalle_ajuste");

                entity.Property(e => e.id_ajuste_inventario).HasColumnName("id_ajuste_inventario");
                entity.Property(e => e.id_producto_almacen).HasColumnName("id_producto_almacen");

                entity.HasOne(d => d.AjusteInventario)
                      .WithMany(a => a.DetallesAjuste)
                      .HasForeignKey(d => d.id_ajuste_inventario)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ProductoAlmacen)
                      .WithMany()
                      .HasForeignKey(d => d.id_producto_almacen)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
