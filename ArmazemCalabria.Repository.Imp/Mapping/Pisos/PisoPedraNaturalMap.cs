using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoPedraNaturalMap : IEntityTypeConfiguration<PisoPedraNatural>
    {
        public void Configure(EntityTypeBuilder<PisoPedraNatural> builder)
        {
            builder.ToTable("tb_pisos_pedra_natural", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.TipoPedra)
                .HasColumnName("tipo_pedra")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.FlagPorosidade)
                .HasColumnName("fl_porosidade")
                .IsRequired();

            builder.Property(p => p.FlagNecessitaImpermeabilizacao)
                .HasColumnName("fl_necessita_impermeabilizacao")
                .IsRequired();
        }
    }
}
