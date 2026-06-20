using ArmazemCalabria.Entity.Entities.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Dominio
{
    public class NivelResistenciaMap : IEntityTypeConfiguration<NivelResistencia>
    {
        public void Configure(EntityTypeBuilder<NivelResistencia> builder)
        {
            builder.ToTable("tb_niveis_resistencia", "armazem");

            builder.HasKey(n => n.IdNivelResistencia);

            builder.Property(n => n.IdNivelResistencia)
                .HasColumnName("id_nivel_resistencia")
                .ValueGeneratedOnAdd();

            builder.Property(n => n.Descricao)
                .HasColumnName("descricao")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(n => n.Pisos)
                .WithOne(p => p.NivelResistencia)
                .HasForeignKey(p => p.IdNivelResistencia)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
