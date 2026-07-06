using ArmazemCalabria.Entity.Entities.Importacao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Importacao
{
    public class ErroImportacaoMap : IEntityTypeConfiguration<ErroImportacao>
    {
        public void Configure(EntityTypeBuilder<ErroImportacao> builder)
        {
            builder.ToTable("tb_erros_importacao", "armazem");

            builder.HasKey(e => e.IdErro);

            builder.Property(e => e.IdErro)
                .HasColumnName("id_erro")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IdArquivo)
                .HasColumnName("id_arquivo")
                .IsRequired();

            builder.Property(e => e.NumeroLinha)
                .HasColumnName("numero_linha")
                .IsRequired();

            builder.Property(e => e.Coluna)
                .HasColumnName("coluna")
                .HasMaxLength(100);

            builder.Property(e => e.Mensagem)
                .HasColumnName("mensagem")
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.ConteudoLinha)
                .HasColumnName("conteudo_linha")
                .HasMaxLength(2000);

            builder.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
