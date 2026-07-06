using ArmazemCalabria.Entity.Entities.Importacao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping.Importacao
{
    public class ArquivoImportacaoMap : IEntityTypeConfiguration<ArquivoImportacao>
    {
        public void Configure(EntityTypeBuilder<ArquivoImportacao> builder)
        {
            builder.ToTable("tb_arquivos_importacao", "armazem");

            builder.HasKey(a => a.IdArquivo);

            builder.Property(a => a.IdArquivo)
                .HasColumnName("id_arquivo")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.NomeOriginal)
                .HasColumnName("nome_original")
                .IsRequired()
                .HasMaxLength(260);

            builder.Property(a => a.ContentType)
                .HasColumnName("content_type")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.TamanhoBytes)
                .HasColumnName("tamanho_bytes")
                .IsRequired();

            builder.Property(a => a.Conteudo)
                .HasColumnName("conteudo")
                .HasColumnType("VARBINARY(MAX)")
                .IsRequired();

            builder.Property(a => a.Status)
                .HasColumnName("id_status")
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(a => a.TotalRegistros)
                .HasColumnName("total_registros")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.TotalSucesso)
                .HasColumnName("total_sucesso")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.TotalErros)
                .HasColumnName("total_erros")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(a => a.MensagemErro)
                .HasColumnName("mensagem_erro")
                .HasMaxLength(1000);

            builder.Property(a => a.IdUsuarioEnvio)
                .HasColumnName("id_usuario_envio")
                .IsRequired();

            builder.Property(a => a.DataEnvio)
                .HasColumnName("data_envio")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.DataProcessamento)
                .HasColumnName("data_processamento");

            builder.HasOne(a => a.UsuarioEnvio)
                .WithMany()
                .HasForeignKey(a => a.IdUsuarioEnvio)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Erros)
                .WithOne(e => e.Arquivo)
                .HasForeignKey(e => e.IdArquivo)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
