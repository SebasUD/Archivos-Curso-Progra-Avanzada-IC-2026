using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Entities;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MyLibrary.DbModel.Context;

public partial class MySqlDbContext : DbContext
{
    public MySqlDbContext()
    {
    }

    public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookauthor> Bookauthors { get; set; }

    public virtual DbSet<Booktheme> Bookthemes { get; set; }

    public virtual DbSet<Bookcopy> Bookcopies { get; set; }

    public virtual DbSet<Borrow> Borrows { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql("name=ConnectionStrings:MySqlDbContext", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("authors")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bio)
                .HasColumnType("text")
                .HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("books")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.Isbn, "isbn").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.Pages).HasColumnName("pages");
            entity.Property(e => e.PublishedDate).HasColumnName("published_date");
            entity.Property(e => e.Publisher)
                .HasMaxLength(200)
                .HasColumnName("publisher");
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Booktheme>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.ThemeId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("bookthemes")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.ThemeId, "fk_bt_theme");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Bookthemes)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("fk_bt_book");

            entity.HasOne(d => d.Theme).WithMany(p => p.Bookthemes)
                .HasForeignKey(d => d.ThemeId)
                .HasConstraintName("fk_bt_theme");
        });

        modelBuilder.Entity<Bookauthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("bookauthors")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.AuthorId, "fk_ba_author");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasDefaultValueSql("'author'")
                .HasColumnName("role");

            entity.HasOne(d => d.Author).WithMany(p => p.Bookauthors)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("fk_ba_author");

            entity.HasOne(d => d.Book).WithMany(p => p.Bookauthors)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("fk_ba_book");
        });

        modelBuilder.Entity<Bookcopy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("bookcopies")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.Barcode, "barcode").IsUnique();

            entity.HasIndex(e => e.BookId, "fk_bookcopies_book");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("added_at");
            entity.Property(e => e.Barcode)
                .HasMaxLength(100)
                .HasColumnName("barcode");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.PenaltyWeight)
                .HasDefaultValueSql("'1'")
                .HasColumnName("penalty_weight");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'2'")
                .HasColumnName("status");

            entity.HasOne(d => d.Book).WithMany(p => p.Bookcopies)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("fk_bookcopies_book");
        });

        modelBuilder.Entity<Borrow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("borrows")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.BookCopyId, "fk_borrows_copy");

            entity.HasIndex(e => e.UserId, "fk_borrows_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookCopyId).HasColumnName("book_copy_id");
            entity.Property(e => e.BorrowDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("borrow_date");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("datetime")
                .HasColumnName("return_date");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.BookCopy).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.BookCopyId)
                .HasConstraintName("fk_borrows_copy");

            entity.HasOne(d => d.User).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_borrows_user");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("themes")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_0900_ai_ci");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.MaxConcurrentBorrows)
                .HasDefaultValueSql("'1'")
                .HasColumnName("max_concurrent_borrows");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
