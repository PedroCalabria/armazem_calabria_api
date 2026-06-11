using ArmazemCalabria.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping
{
    public class PerfilMap : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.ToTable("tb_perfis", "usuario");

            builder.HasKey(p => p.IdPerfil);
    
            builder.Property(p => p.IdPerfil)
                .HasColumnName("id_perfil")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Descricao)
                .HasColumnName("descricao")
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
