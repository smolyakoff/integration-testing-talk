using BookShop.Application.Features.Catalog;
using BookShop.Application.Features.OrderProcessing;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace BookShop.Application.Infrastructure.Database
{
    internal class BookShopDbContext : DbContext
    {
        public BookShopDbContext(DbContextOptions<BookShopDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; private set; }
        public DbSet<Cart> Carts { get; private set; }
        public DbSet<Order> Orders { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderLine>().Property("_bookId");
            modelBuilder.Entity<OrderLine>().HasKey(nameof(OrderLine.OrderId), "_bookId");
        }
    }
}