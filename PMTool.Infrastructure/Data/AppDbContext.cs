using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectBacklog> ProjectBacklogs { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.HasIndex(e => e.RoleType)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<RolePermission>(
                    j => j.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId),
                    j => j.HasOne(rp => rp.Role).WithMany().HasForeignKey(rp => rp.RoleId));

            entity.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.HasIndex(e => e.PermissionType)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.UserId, e.RoleId, e.ProjectId })
                .IsUnique();
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId })
                .IsUnique();
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.ClientName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ProjectCode)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(e => e.ProjectCode)
                .IsUnique();

            entity.Property(e => e.ColourCode)
                .HasMaxLength(7); // Hex colour code

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(p => p.Backlogs)
                .WithOne(pb => pb.Project)
                .HasForeignKey(pb => pb.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProjectBacklog configuration
        modelBuilder.Entity<ProjectBacklog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(pb => pb.Project)
                .WithMany(p => p.Backlogs)
                .HasForeignKey(pb => pb.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Team configuration
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasMany(t => t.TeamMembers)
                .WithOne(tm => tm.Team)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TeamMember configuration
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.TeamId, e.UserId })
                .IsUnique();

            entity.HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Extended User configuration for Team relationship
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.TeamMembers)
                .WithOne(tm => tm.User)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
