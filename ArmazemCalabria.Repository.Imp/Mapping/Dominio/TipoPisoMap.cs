using ArmazemCalabria.Entity.Entities.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Dominio
{
    public class TipoPisoMap : IEntityTypeConfiguration<TipoPiso>
    {
        public void Configure(EntityTypeBuilder<TipoPiso> builder)
        {
            builder.ToTable("tb_tipos_piso", "armazem");

            builder.HasKey(t => t.IdTipoPiso);

            builder.Property(t => t.IdTipoPiso)
                .HasColumnName("id_tipo_piso")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Descricao)
                .HasColumnName("descricao")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(t => t.Pisos)
                .WithOne(p => p.TipoPiso)
                .HasForeignKey(p => p.IdTipoPiso)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
