using ArmazemCalabria.Entity.Entities;
using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;

namespace ArmazemCalabria.Repository
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Piso> Pisos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
