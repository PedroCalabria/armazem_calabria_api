using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoCeramicaMap : IEntityTypeConfiguration<PisoCeramica>
    {
        public void Configure(EntityTypeBuilder<PisoCeramica> builder)
        {
            builder.ToTable("tb_pisos_ceramica", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.ClassePei)
                .HasColumnName("classe_pei")
                .IsRequired();
        }
    }
}
