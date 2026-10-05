using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NeruNook.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ContactInquiry>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120);
            entity.Property(x => x.Email).HasMaxLength(254);
            entity.Property(x => x.Company).HasMaxLength(160);
            entity.Property(x => x.ProjectType).HasMaxLength(80);
            entity.Property(x => x.Budget).HasMaxLength(80);
            entity.Property(x => x.Message).HasMaxLength(4000);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasIndex(x => x.CreatedAtUtc);
            entity.HasIndex(x => x.Status);
        });
    }
}
