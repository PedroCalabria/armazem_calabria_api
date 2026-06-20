using ArmazemCalabria.Entity.Entities.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Dominio
{
    public class MarcaMap : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.ToTable("tb_marcas", "armazem");

            builder.HasKey(m => m.IdMarca);

            builder.Property(m => m.IdMarca)
                .HasColumnName("id_marca")
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Nome)
                .HasColumnName("nome")
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(m => m.Pisos)
                .WithOne(p => p.Marca)
                .HasForeignKey(p => p.IdMarca)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
