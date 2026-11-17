using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;

namespace TranHoangKhai_25DH190104_BE.Models
{
    // Lop DbContext trung ten theo DB-First: MyStoreEntities.
    // Neu co giao dung DB-First (.edmx), thay the file nay bang model sinh tu EDMX.
    public class MyStoreEntities : DbContext
    {
        public MyStoreEntities() : base("name=MyStoreEntities") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }

    [Table("User")]
    public class User
    {
        [Key] public string Username { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; } // A: admin, C: customer
        public virtual ICollection<Customer> Customers { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Username { get; set; }
        [ForeignKey("Username")] public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public string ProductDecription { get; set; } // ten cot dung theo de bai
        public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        [ForeignKey("CategoryID")] public virtual Category Category { get; set; }
        [NotMapped] public HttpPostedFileBase UploadImg { get; set; } // upload anh, khong luu DB
    }

    [Table("Order")]
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingAddress { get; set; } // da doi ten tu AddressDelivery
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        [ForeignKey("CustomerID")] public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalPrice { get; set; } // cot tinh Quantity*UnitPrice (PERSISTED)
        [ForeignKey("ProductID")] public virtual Product Product { get; set; }
        [ForeignKey("OrderID")] public virtual Order Order { get; set; }
    }
}