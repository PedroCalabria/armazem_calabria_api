using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoLaminadoMap : IEntityTypeConfiguration<PisoLaminado>
    {
        public void Configure(EntityTypeBuilder<PisoLaminado> builder)
        {
            builder.ToTable("tb_pisos_laminado", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.FlagResistenteCupim)
                .HasColumnName("fl_resistente_cupim")
                .IsRequired();
        }
    }
}
