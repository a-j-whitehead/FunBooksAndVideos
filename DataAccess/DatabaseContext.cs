using DataAccess.DataModels;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class DatabaseContext : DbContext
    {
        private const string _localSqlServerName = "localhost\\SQLEXPRESS";

        public DatabaseContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer($"Server={_localSqlServerName};Database=FunBooksAndVideos;Trusted_Connection=True;TrustServerCertificate=True;");

        public void InitialSetup()
        {
            if (Database.EnsureCreated())
            {
                Set<Product>().AddRange(
                [
                    new Product("Comprehensive First Aid Training", ProductType.Video),
                    new Product("The Girl on the train", ProductType.Book),
                    new Product("Book Club Membership", ProductType.BookMembership),
                    new Product("Video Club Membership", ProductType.VideoMembership)
                ]);

                Set<Customer>().Add(new Customer());

                SaveChanges();
            }
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SetUpModels(modelBuilder);
        }

        private static void SetUpModels(ModelBuilder modelBuilder)
        {
            SetUpCustomer(modelBuilder);

            SetUpProduct(modelBuilder);

            SetUpPurchaseOrder(modelBuilder);
        }

        private static void SetUpCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>()
                .Property(x => x.IsBookMember);

            modelBuilder.Entity<Customer>()
                .Property(x => x.IsVideoMember);

            modelBuilder.Entity<Customer>()
                .HasMany(x => x.PurchaseOrders)
                .WithOne(x => x.Customer);
        }

        private static void SetUpProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(x => x.ProductId);

            modelBuilder.Entity<Product>()
                .Property(x => x.Type);

            modelBuilder.Entity<Product>()
                .Property(x => x.Name);
        }

        private static void SetUpPurchaseOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>()
                .Property(x => x.PurchaseOrderId)
                .ValueGeneratedNever();

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.PurchaseOrders);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(x => x.Total)
                .HasPrecision(9, 2); //TODO - move this to shared file?

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(x => x.Products)
                .WithMany(x => x.PurchaseOrders)
                .UsingEntity<PurchaseOrderProduct>();
        }
    }
}