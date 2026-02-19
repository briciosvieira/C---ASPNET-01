using Microsoft.EntityFrameworkCore;
using shopping.Models;

namespace shopping.Data;

public class ToDoContext : DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options)
        : base(options)
    {
    }

    public DbSet<ToDoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoItem>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.id);
            
            // Configurar propriedades
            entity.Property(e => e.title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.description)
                .HasMaxLength(1000);
            
            entity.Property(e => e.createdAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.isCompleted)
                .HasDefaultValue(false);
        });
    }
}