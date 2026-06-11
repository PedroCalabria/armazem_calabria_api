using ArmazemCalabria.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmazemCalabria.Repository.Imp.Mapping
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("tb_usuarios", "usuario");

            builder.HasKey(p => p.IdUsuario);

            builder.Property(p => p.IdUsuario)
                .HasColumnName("id_usuario")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nome)
                .HasColumnName("nome")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Senha)
                .HasColumnName("senha")
                .IsRequired()
                .HasMaxLength(255);


            builder.Property(p => p.FlagAprovado)
                .HasColumnName("fl_aprovado")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.DataAprovacao)
                .HasColumnName("data_aprovacao");

            builder.Property(p => p.IdUsuarioAlteracao)
                .HasColumnName("id_usuario_alteracao");

            builder.Property(p => p.DataAlteracao)
                .HasColumnName("data_alteracao");


            builder.Property(p => p.IdPerfil)
                .HasColumnName("id_perfil")
                .IsRequired();

            builder.HasOne(u => u.Perfil)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.IdPerfil)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(p => p.IdUsuarioAprovador)
                .HasColumnName("id_usuario_aprovador");

            builder.HasOne(u => u.UsuarioAprovador)
                .WithMany()
                .HasForeignKey(u => u.IdUsuarioAprovador)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
