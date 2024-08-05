    using System;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LibraryAPI.Data
{
	public class ApplicationContext: IdentityDbContext<AppUser>
	{
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        public DbSet<Location>? Locations { get; set; }
        public DbSet<Language>? Languages { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<SubCategory>? SubCategories { get; set; }
        public DbSet<Publisher>? Publishers { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<Book>? Books { get; set; }
        public DbSet<AuthorBook >? AuthorBook { get; set; }
        public DbSet<BookLanguage>? BookLanguage { get; set; }
        public DbSet<BookSubCategory>? BookSubCategory { get; set; }
        public DbSet<Member>? Members { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<BookCopy>? BookCopies { get; set; }
        public DbSet<BookCheckOut>? BookCheckOuts { get; set; }
        
        
       
        
        public DbSet<PunishmentInvoice>? PunishmentInvoices{ get; set; }
        public DbSet<PunishmentReason>? PunishmentReasons{ get; set; }
     
        public DbSet<Vote>? Votes { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<AuthorBook>().HasKey(a => new { a.AuthorsId, a.BooksID });

            modelBuilder.Entity<BookLanguage>().HasKey(b => new { b.BooksID, b.LanguagesCode });

            modelBuilder.Entity<BookSubCategory>().HasKey(s => new { s.BooksID, s.SubCategoryId });

            modelBuilder.Entity<Vote>().HasKey(v => new {v.BookId , v.MemberId });

            modelBuilder.Entity<BookCheckOut>()
        .HasIndex(e => e.BookCopyID);

            modelBuilder.Entity<Vote>()
            .HasOne(v => v.Book)
            .WithMany(v=>v.Votes) // Adjust according to your Book entity configuration
            .HasForeignKey(v => v.BookId);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Member)
                .WithMany(v=>v.Votes) // Adjust according to your Member entity configuration
                .HasForeignKey(v => v.MemberId);

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<BookSubCategory>()
            //    .HasOne(ab => ab.SubCategory).WithMany(a => a.BookSubCategories).HasForeignKey(ab => ab.SubCategoryId);
            //modelBuilder.Entity<BookSubCategory>()
            //    .HasOne(ab => ab.Book).WithMany(a => a.BookSubCategories).HasForeignKey(ab => ab.BooksID);



            //modelBuilder.Entity<BookLanguage>()
            //.HasOne(ab => ab.Language).WithMany(a => a.BookLanguages).HasForeignKey(ab => ab.LanguagesCode);

            //modelBuilder.Entity<BookLanguage>()
            //    .HasOne(ab => ab.Book).WithMany(a => a.BookLanguages).HasForeignKey(ab => ab.BooksID);


            //modelBuilder.Entity<AuthorBook>()
            //.HasOne(ab => ab.Author)
            //.WithMany(a => a.AuthorBooks)
            //.HasForeignKey(ab => ab.AuthorsId);

            //modelBuilder.Entity<AuthorBook>()
            //.HasOne(ab => ab.Book)
            //.WithMany(b => b.AuthorBooks)
            //.HasForeignKey(ab => ab.BooksID);
        }

    }
}

