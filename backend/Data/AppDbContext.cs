using Microsoft.EntityFrameworkCore;
using SmartLifePlanner.Models;
namespace SmartLifePlanner.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<SmartLifePlanner.Models.Task> Tasks { get; set; } = null!;
    public DbSet<StudyGoal> StudyGoals { get; set; } = null!;
    public DbSet<WorkDay> WorkDays { get; set; } = null!;
    public DbSet<IncomeGoal> IncomeGoals { get; set; } = null!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Subjects)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);
        
        

        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Tasks)
            .WithOne(t => t.Subject)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.StudyGoals)
            .WithOne(g => g.User)
            .HasForeignKey(g => g.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.WorkDays)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasMany(u => u.IncomeGoals)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RefreshToken>()
            .HasOne<User>(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
        modelBuilder.Entity<IncomeGoal>()
            .Property(i => i.TargetAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<IncomeGoal>()
            .Property(i => i.CurrentAmount)
            .HasPrecision(18, 2);
    }
}