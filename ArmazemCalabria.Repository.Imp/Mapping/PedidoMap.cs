using ArmazemCalabria.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("tb_pedidos", "armazem");

            builder.HasKey(p => p.IdPedido);

            builder.Property(p => p.IdPedido)
                .HasColumnName("id_pedido")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.IdUsuarioSolicitante)
                .HasColumnName("id_usuario_solicitante")
                .IsRequired();

            builder.Property(p => p.IdStatus)
                .HasColumnName("id_status")
                .IsRequired()
                .HasConversion<byte>();

            builder.Property(p => p.MotivoRejeicao)
                .HasColumnName("motivo_rejeicao")
                .HasMaxLength(500);

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("data_alteracao");

            builder.Property(p => p.IdUsuarioAprovador)
                .HasColumnName("id_usuario_aprovador");

            builder.Property(p => p.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(p => p.DataRejeicao)
                .HasColumnName("data_rejeicao");

            builder.HasOne(p => p.UsuarioSolicitante)
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioSolicitante)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UsuarioAprovador)
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioAprovador)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.IdPedido)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
