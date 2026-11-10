using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Entities;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace Store.Infrastructure.Data;
/// <summary>Represents the application's Entity Framework Core database session.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Photo> Photos { get; set; }

    public override int SaveChanges()
    {
        ApplyAuditMetadata();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditMetadata();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "entity");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyAuditMetadata()
    {
        var timestamp = DateTime.UtcNow;
        var auditUser = GetCurrentUserIdentifier();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable auditable)
                continue;

            if (entry.State == EntityState.Added)
            {
                auditable.CreatedAt = auditable.CreatedAt == default ? timestamp : auditable.CreatedAt;
                auditable.CreatedBy ??= auditUser;
                auditable.UpdatedAt ??= auditable.CreatedAt;
                auditable.UpdatedBy ??= auditable.CreatedBy;
            }

            if (entry.State == EntityState.Modified)
            {
                auditable.UpdatedAt = timestamp;
                auditable.UpdatedBy = auditUser;
            }

            if (entry.Entity is ISoftDeletable softDeletable)
            {
                if (softDeletable.IsDeleted)
                {
                    softDeletable.DeletedAt ??= timestamp;
                    softDeletable.DeletedBy ??= auditUser;
                }
                else
                {
                    softDeletable.DeletedAt = null;
                    softDeletable.DeletedBy = null;
                }
            }
        }
    }

    private static string GetCurrentUserIdentifier()
    {
        var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
        var identityName = principal?.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(identityName))
            return identityName;

        var claim = principal?.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier || c.Type == ClaimTypes.Name || c.Type == "sub");

        return claim?.Value ?? "system";
    }
}
