using APIUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            b.Property(u => u.Email).IsRequired();
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Senha).IsRequired();
            b.Property(u => u.DataCriacao).IsRequired();
        });
    }
}