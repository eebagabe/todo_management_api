using Microsoft.EntityFrameworkCore;
using TodoManagement.Api.Models;

namespace TodoManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().Property(t => t.EstimatedHours).HasPrecision(8, 2);
        modelBuilder.Entity<TaskItem>().Property(t => t.ActualHours).HasPrecision(8, 2);

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.Labels)
            .WithMany()
            .UsingEntity("label_task",
                l => l.HasOne(typeof(Label)).WithMany().HasForeignKey("LabelId"),
                t => t.HasOne(typeof(TaskItem)).WithMany().HasForeignKey("TaskId"));

        modelBuilder.Entity<Label>().HasIndex(l => l.Name).IsUnique();
    }
}
