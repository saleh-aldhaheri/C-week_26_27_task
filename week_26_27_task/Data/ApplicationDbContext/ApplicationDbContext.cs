
using Microsoft.EntityFrameworkCore;

namespace week_26_27_task.Data.ApplicationDbContext
{
    public class ApplicationDbContext : DbContext
    {
        DbSet<Actor> Actors { get; set; } = null!;
        DbSet<Movie> Movies { get; set; } = null!;
        DbSet<MovieActor> MovieActors { get; set; } = null!;
        DbSet<Cinema> Cinemas { get; set; } = null!;
        DbSet<Category> Categories { get; set; } = null!;
        DbSet<MovieSubImg> MovieSubImgs { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .Property(e => e.Title)
                .HasMaxLength(100);

            modelBuilder.Entity<Movie>()
                .HasIndex(e => e.Title);

            modelBuilder.Entity<Category>()
              .Property(e => e.Name)
              .HasMaxLength(100);

            modelBuilder.Entity<Category>()
              .HasIndex(e => e.Name)
              .IsUnique();

            modelBuilder.Entity<Cinema>()
              .Property(e => e.Name)
              .HasMaxLength(100);  

            modelBuilder.Entity<Cinema>()
              .Property(e => e.Location)
              .HasMaxLength(1000);

            modelBuilder.Entity<Cinema>()
              .HasIndex(e => e.Name);

            modelBuilder.Entity<Actor>()
                .Property(e => e.FullName)
                .HasMaxLength(100);

            modelBuilder.Entity<Actor>()
                .HasIndex(e => e.FullName);

            modelBuilder.Entity<MovieActor>()
                .HasIndex(ma => new { ma.MovieId, ma.ActorId })
                .IsUnique(true);
        }
    }
}

