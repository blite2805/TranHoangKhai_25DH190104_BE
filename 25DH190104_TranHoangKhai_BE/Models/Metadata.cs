using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // Metadata classes gắn validation cho các entity generated từ EDMX (Customer/Product/Category).
    // Các class metadata khai báo lại tên property (KHÔNG trùng với partial class bên dưới — partial chỉ gắn [MetadataType]).
    // Áp dụng điều kiện validation: kiểm tra độ dài [StringLength]/[Required] cho mọi trường text.

    internal sealed class CustomerMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, MinimumLength = 10)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ gồm chữ số")]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string CustomerEmail { get; set; }

        [StringLength(200)]
        public string CustomerAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }
    }

    internal sealed class ProductMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải >= 0")]
        public decimal ProductPrice { get; set; }

        [StringLength(500)]
        public string ProductImage { get; set; }

        [StringLength(1000)]
        public string ProductDescription { get; set; }
    }

    internal sealed class CategoryMetadata
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100)]
        public string CategoryName { get; set; }
    }

    // Partial class chỉ gắn [MetadataType], KHÔNG khai báo lại property (tránh trùng với class generated)
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
    }

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
    }

    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {
    }
}
