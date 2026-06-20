using ArmazemCalabria.Entity.Entities.Pisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Pisos
{
    public class PisoMap : IEntityTypeConfiguration<Piso>
    {
        public void Configure(EntityTypeBuilder<Piso> builder)
        {
            builder.ToTable("tb_pisos", "armazem");
            builder.UseTptMappingStrategy();

            builder.HasKey(p => p.IdPiso);

            builder.Property(p => p.IdPiso)
                .HasColumnName("id_piso")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nome)
                .HasColumnName("nome")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Cor)
                .HasColumnName("cor")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Preco)
                .HasColumnName("preco")
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Largura)
                .HasColumnName("largura")
                .IsRequired()
                .HasPrecision(8, 2);

            builder.Property(p => p.Comprimento)
                .HasColumnName("comprimento")
                .IsRequired()
                .HasPrecision(8, 2);

            builder.Property(p => p.Espessura)
                .HasColumnName("espessura")
                .IsRequired()
                .HasPrecision(6, 2);

            builder.Property(p => p.Peso)
                .HasColumnName("peso")
                .IsRequired()
                .HasPrecision(8, 2);

            builder.Property(p => p.FlagResistenteAgua)
                .HasColumnName("fl_resistente_agua")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.FlagAntiderrapante)
                .HasColumnName("fl_antiderrapante")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.QuantidadeDisponivel)
                .HasColumnName("quantidade_disponivel")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.IdTipoPiso)
                .HasColumnName("id_tipo_piso")
                .IsRequired();

            builder.Property(p => p.IdMarca)
                .HasColumnName("id_marca")
                .IsRequired();

            builder.Property(p => p.IdNivelResistencia)
                .HasColumnName("id_nivel_resistencia")
                .IsRequired();

            builder.Property(p => p.IdAcabamento)
                .HasColumnName("id_acabamento")
                .IsRequired();

            builder.Property(p => p.IdAmbiente)
                .HasColumnName("id_ambiente")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.IdUsuarioCriacao)
                .HasColumnName("id_usuario_criacao")
                .IsRequired();

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("data_alteracao");

            builder.Property(p => p.IdUsuarioAlteracao)
                .HasColumnName("id_usuario_alteracao");

            builder.HasOne(p => p.UsuarioCriacao)
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioCriacao)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UsuarioAlteracao)
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioAlteracao)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
