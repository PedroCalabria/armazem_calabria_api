using ArmazemCalabria.Entity.Entities.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Dominio
{
    public class AmbienteMap : IEntityTypeConfiguration<Ambiente>
    {
        public void Configure(EntityTypeBuilder<Ambiente> builder)
        {
            builder.ToTable("tb_ambientes", "armazem");

            builder.HasKey(a => a.IdAmbiente);

            builder.Property(a => a.IdAmbiente)
                .HasColumnName("id_ambiente")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Descricao)
                .HasColumnName("descricao")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(a => a.Pisos)
                .WithOne(p => p.Ambiente)
                .HasForeignKey(p => p.IdAmbiente)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
