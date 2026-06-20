using ArmazemCalabria.Entity.Entities.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Dominio
{
    public class AcabamentoMap : IEntityTypeConfiguration<Acabamento>
    {
        public void Configure(EntityTypeBuilder<Acabamento> builder)
        {
            builder.ToTable("tb_acabamentos", "armazem");

            builder.HasKey(a => a.IdAcabamento);

            builder.Property(a => a.IdAcabamento)
                .HasColumnName("id_acabamento")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Descricao)
                .HasColumnName("descricao")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(a => a.Pisos)
                .WithOne(p => p.Acabamento)
                .HasForeignKey(p => p.IdAcabamento)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
