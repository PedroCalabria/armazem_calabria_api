using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Entity.Entities.Importacao;
using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Piso> Pisos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidosItens { get; set; }
        public DbSet<ArquivoImportacao> ArquivosImportacao { get; set; }
        public DbSet<ErroImportacao> ErrosImportacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
