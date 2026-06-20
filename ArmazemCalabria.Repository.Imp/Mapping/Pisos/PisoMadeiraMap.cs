using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoMadeiraMap : IEntityTypeConfiguration<PisoMadeira>
    {
        public void Configure(EntityTypeBuilder<PisoMadeira> builder)
        {
            builder.ToTable("tb_pisos_madeira", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.TipoMadeira)
                .HasColumnName("tipo_madeira")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.FlagMadeiraNobre)
                .HasColumnName("fl_madeira_nobre")
                .IsRequired();
        }
    }
}
