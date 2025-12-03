using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace VP_Lab_11_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnViewPurchases_Click(object sender, RoutedEventArgs e)
        {
            using var db = new SmartMartContext();
            var result = (from c in db.Customer
                         join sr in db.SalesRecord on c.CustomerID equals sr.CustomerID
                         join p in db.Product on sr.ProductID equals p.ProductID
                         select new
                         {
                             Customer_Name = c.Name,
                             City = c.City,
                             Product_Name = p.ProductName,
                             Sale_Date = sr.SaleDate
                         }).ToList();
            dgResults.ItemsSource = result;
        }

        private void btnNoPurchases_Click(object sender, RoutedEventArgs e)
        {
            using var db = new SmartMartContext();
            var result = (from c in db.Customer
                          join sr in db.SalesRecord on c.CustomerID equals sr.CustomerID into NoPurchase
                          from np in NoPurchase.DefaultIfEmpty() where np == null
                          select new { c.Name, c.City, ProductName = "No Product" }).ToList();
            dgResults.ItemsSource = result;
        }

        private void btnSalesPerCustomer_Click(object sender, RoutedEventArgs e)
        {
            using var db = new SmartMartContext();
            var result = (from c in db.Customer
                          join sr in db.SalesRecord on c.CustomerID equals sr.CustomerID into SalesPerCustomer
                          select new
                          {
                              c.Name,
                              TotalProductsPurchased = SalesPerCustomer.Count()
                          }).ToList();
            dgResults.ItemsSource = result;
        }

        private void btnSalesPerCategory_Click(object sender, RoutedEventArgs e)
        {
            using var db = new SmartMartContext();
            var result = (from c in db.Product.Select(p => p.Category).Distinct()
                          join p in db.Product on c equals p.Category into SalesPerCategory
                          select new
                          {
                              Category = c,
                              TotalSales = SalesPerCategory.SelectMany(pg => pg.SalesRecord).Count()
                          }).ToList();
            dgResults.ItemsSource = result;
        }
    }
    public class Customer
    {
        public int CustomerID { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public ICollection<SalesRecord> SalesRecord { get; set; }
    }
    public class Product
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public ICollection<SalesRecord> SalesRecord { get; set; }
    }
    public class SalesRecord
    {
        public int SaleID { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public DateTime? SaleDate { get; set; }
    }
    public class SmartMartContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<SalesRecord> SalesRecord { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseSqlServer(@"Server = (localdb)\ProjectModels; Database = SmartMartDB; Trusted_Connection = True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define Primary Keys
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerID);

            modelBuilder.Entity<Product>().HasKey(p => p.ProductID);

            modelBuilder.Entity<SalesRecord>().HasKey(sr => sr.SaleID);

            // Define Non-Increment Behaviour
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .ValueGeneratedNever();

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductID)
                .ValueGeneratedNever();

            modelBuilder.Entity<SalesRecord>()
                .Property(sr => sr.SaleID)
                .ValueGeneratedNever();

            // Define Foreign Keys
            //Link Sales Record with Customer
            modelBuilder.Entity<SalesRecord>()
                .HasOne(sr => sr.Customer)
                .WithMany(c => c.SalesRecord)
                .HasForeignKey(sr => sr.CustomerID);

            //Link Sales Record with Product
            modelBuilder.Entity<SalesRecord>()
                .HasOne(sr => sr.Product)
                .WithMany(p => p.SalesRecord)
                .HasForeignKey(sr => sr.ProductID);

            base.OnModelCreating(modelBuilder);
        }

    }
}