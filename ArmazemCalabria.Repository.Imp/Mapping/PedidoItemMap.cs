using ArmazemCalabria.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArmazemCalabria.Repository.Imp.Mapping
{
    public class PedidoItemMap : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("tb_pedidos_itens", "armazem");

            builder.HasKey(i => i.IdPedidoItem);

            builder.Property(i => i.IdPedidoItem)
                .HasColumnName("id_pedido_item")
                .ValueGeneratedOnAdd();

            builder.Property(i => i.IdPedido)
                .HasColumnName("id_pedido")
                .IsRequired();

            builder.Property(i => i.IdPiso)
                .HasColumnName("id_piso")
                .IsRequired();

            builder.Property(i => i.Quantidade)
                .HasColumnName("quantidade")
                .IsRequired();

            builder.HasOne(i => i.Piso)
                .WithMany()
                .HasForeignKey(i => i.IdPiso)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
