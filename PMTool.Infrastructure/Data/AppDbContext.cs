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
    public DbSet<ProjectDocument> ProjectDocuments { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ReleaseNotes> ReleaseNotes { get; set; } = null!;
    public DbSet<SubProject> SubProjects { get; set; } = null!;
    public DbSet<SubProjectDependency> SubProjectDependencies { get; set; } = null!;
    public DbSet<SubProjectTeam> SubProjectTeams { get; set; } = null!;
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

            entity.HasMany(p => p.Products)
                .WithOne(pr => pr.Project)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Backlogs)
                .WithOne(pb => pb.Project)
                .HasForeignKey(pb => pb.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(p => p.Documents)
                .WithOne(d => d.Project)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProjectDocument configuration
        modelBuilder.Entity<ProjectDocument>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DocumentName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.SubmittedByUser)
                .WithMany()
                .HasForeignKey(d => d.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.VersionName)
                .IsRequired()
                .HasMaxLength(50); // e.g., "1.0.0", "2.5.3"

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.ProjectId, e.VersionName })
                .IsUnique(); // Version name unique per project

            entity.HasOne(p => p.Project)
                .WithMany(pr => pr.Products)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.ReleaseNotes)
                .WithOne(rn => rn.Product)
                .HasForeignKey(rn => rn.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Backlogs)
                .WithOne(pb => pb.Product)
                .HasForeignKey(pb => pb.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
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
                .OnDelete(DeleteBehavior.NoAction); // Changed to NoAction to avoid cascade cycles

            entity.HasOne(pb => pb.Product)
                .WithMany(p => p.Backlogs)
                .HasForeignKey(pb => pb.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(pb => pb.Owner)
                .WithMany()
                .HasForeignKey(pb => pb.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ReleaseNotes configuration
        modelBuilder.Entity<ReleaseNotes>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(rn => rn.Product)
                .WithMany(p => p.ReleaseNotes)
                .HasForeignKey(rn => rn.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rn => rn.CreatedByUser)
                .WithMany()
                .HasForeignKey(rn => rn.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user if release notes exist
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

        // SubProject configuration
        modelBuilder.Entity<SubProject>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200); // e.g., "Student Registration Portal"

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.ColorCode)
                .HasMaxLength(7);

            entity.Property(e => e.Progress)
                .HasDefaultValue(0); // 0-100 based on ticket completion

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(sp => sp.Product)
                .WithMany(p => p.SubProjects)
                .HasForeignKey(sp => sp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sp => sp.ModuleOwner)
                .WithMany()
                .HasForeignKey(sp => sp.ModuleOwnerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user if they own sub-projects

            entity.HasMany(sp => sp.SubProjectTeams)
                .WithOne(spt => spt.SubProject)
                .HasForeignKey(spt => spt.SubProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(sp => sp.Backlog)
                .WithOne(pb => pb.SubProject)
                .HasForeignKey(pb => pb.SubProjectId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade if sub-project deleted

            entity.HasMany(sp => sp.DependsOn)
                .WithOne(sd => sd.SubProject)
                .HasForeignKey(sd => sd.SubProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(sp => sp.DependentOn)
                .WithOne(sd => sd.DependsOnSubProject)
                .HasForeignKey(sd => sd.DependsOnSubProjectId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade from dependent sub-projects
        });

        // SubProjectDependency configuration
        modelBuilder.Entity<SubProjectDependency>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(sd => sd.SubProject)
                .WithMany(sp => sp.DependsOn)
                .HasForeignKey(sd => sd.SubProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sd => sd.DependsOnSubProject)
                .WithMany(sp => sp.DependentOn)
                .HasForeignKey(sd => sd.DependsOnSubProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => new { e.SubProjectId, e.DependsOnSubProjectId })
                .IsUnique(); // Prevent duplicate dependencies
        });

        // SubProjectTeam configuration
        modelBuilder.Entity<SubProjectTeam>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Role)
                .HasMaxLength(100); // e.g., "Development", "QA", "BA"

            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(spt => spt.SubProject)
                .WithMany(sp => sp.SubProjectTeams)
                .HasForeignKey(spt => spt.SubProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(spt => spt.Team)
                .WithMany()
                .HasForeignKey(spt => spt.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.SubProjectId, e.TeamId })
                .IsUnique(); // Prevent assigning same team twice
        });
    }
}
