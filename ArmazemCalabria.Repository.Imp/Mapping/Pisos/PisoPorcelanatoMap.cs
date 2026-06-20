using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoPorcelanatoMap : IEntityTypeConfiguration<PisoPorcelanato>
    {
        public void Configure(EntityTypeBuilder<PisoPorcelanato> builder)
        {
            builder.ToTable("tb_pisos_porcelanato", "armazem");

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso");

            builder.Property(p => p.FlagRetificado)
                .HasColumnName("fl_retificado")
                .IsRequired();

            builder.Property(p => p.TipoPorcelanato)
                .HasColumnName("tipo_porcelanato")
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
