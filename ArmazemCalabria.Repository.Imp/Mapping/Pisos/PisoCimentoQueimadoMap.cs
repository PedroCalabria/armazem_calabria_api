using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoCimentoQueimadoMap : IEntityTypeConfiguration<PisoCimentoQueimado>
    {
        public void Configure(EntityTypeBuilder<PisoCimentoQueimado> builder)
        {
            builder.ToTable("tb_pisos_cimento_queimado", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");
        }
    }
}
