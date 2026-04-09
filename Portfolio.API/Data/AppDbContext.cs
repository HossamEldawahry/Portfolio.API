namespace Portfolio.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasIndex(x => x.Username).IsUnique();
                entity.Property(x => x.Username).HasMaxLength(100);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(x => x.TokenHash).IsUnique();
                entity.HasIndex(x => x.AdminUserId);
                entity.Property(x => x.TokenHash).HasMaxLength(128);
                entity.Property(x => x.JwtId).HasMaxLength(100);
                entity.HasOne(x => x.AdminUser)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.AdminUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
