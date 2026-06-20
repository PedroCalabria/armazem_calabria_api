using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoVinilicoMap : IEntityTypeConfiguration<PisoVinilico>
    {
        public void Configure(EntityTypeBuilder<PisoVinilico> builder)
        {
            builder.ToTable("tb_pisos_vinilico", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.FlagAcustico)
                .HasColumnName("fl_acustico")
                .IsRequired();

            builder.Property(p => p.TipoInstalacao)
                .HasColumnName("tipo_instalacao")
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
