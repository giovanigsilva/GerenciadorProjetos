using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<HistoricoTarefa> HistoricoTarefas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.UsuarioID);

                entity.Property(u => u.DataCriacao)
                      .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            modelBuilder.Entity<Projeto>(entity =>
            {
                entity.HasKey(p => p.ProjetoID);

                entity.Property(p => p.ProjetoID)
                      .UseIdentityColumn();

                entity.Property(p => p.DataCriacao)
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(p => p.Usuario)
                      .WithMany(u => u.Projetos)
                      .HasForeignKey(p => p.UsuarioID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.HasKey(t => t.TarefaID);

                entity.Property(p => p.DataCriacao)
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(t => t.Projeto)
                      .WithMany(p => p.Tarefas)
                      .HasForeignKey(t => t.ProjetoID)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<HistoricoTarefa>()
                .HasOne(h => h.Tarefa)
                .WithMany(t => t.Historico)
                .HasForeignKey(h => h.TarefaID)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<HistoricoTarefa>(entity =>
            {
                entity.HasKey(h => h.HistoricoID);

                entity.HasOne(h => h.Tarefa)
                      .WithMany(t => t.Historico)
                      .HasForeignKey(h => h.TarefaID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.Usuario)
                      .WithMany()
                      .HasForeignKey(h => h.UsuarioID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tarefa>()
                        .ToTable("Tarefas", tb =>
                        {
                            tb.HasTrigger("trg_HistoricoTarefas_Atualizacao");
                            tb.HasTrigger("trg_Tarefas_NaoAlterarPrioridade");
                        });

        }
    }
}
