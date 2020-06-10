using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Core.Models
{
    public sealed class BookLibraryContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Book> Books {get; set;}
        public DbSet<Category> BookCategories {get; set;}
        public DbSet<Borrower> Borrowers {get; set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // book has many borrowers
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Borrowers)
                .WithOne(e => e.Book);
            // category has many books
            modelBuilder.Entity<Category>()
               .HasMany(b => b.Books)
               .WithOne(e => e.Category);
            //borrower haone book
            //modelBuilder.Entity<Borrower>()
            //    .HasOne(b => b.Book)
            //    .WithMany(c => c.Borrowers);
            // enum value converter for the BorrowStatus
            modelBuilder
            .Entity<Borrower>()
            .Property(e => e.BorrowStatus)
            .HasConversion(
                v => v.ToString(),
                v => (BorrowStatus)Enum.Parse(typeof(BorrowStatus), v));
            // enum value converter for the UserStatus
            modelBuilder
            .Entity<ApplicationUser>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (UserStatus)Enum.Parse(typeof(UserStatus), v));
            // set extend count to default zero 
            // this field will be increased if the borrowed book is extended 
            modelBuilder
                .Entity<Borrower>()
                .Property(e => e.ExtendCount)
                .HasDefaultValue(0);

        }
        // to save the created or modified date with each entry 
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            addedEntities.ForEach(E =>
            {
                E.Property("Created").CurrentValue = DateTime.Now;
            });

            var editedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            editedEntities.ForEach(E =>
            {
                E.Property("Modified").CurrentValue = DateTime.Now;
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }

}
