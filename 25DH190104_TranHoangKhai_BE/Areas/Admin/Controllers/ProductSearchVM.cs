using _25DH190104_TranHoangKhai_BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace _25DH190104_TranHoangKhai_BE.Areas.Admin.Controllers
{
    public class ProductSearchVM
    {
        // Tiêu chí tìm kiếm theo tên, mô tả hoặc loại sản phẩm
        public string SearchTerm { get; set; }

        // Tiêu chí tìm kiếm theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp
        public string SortOrder { get; set; }

        // Phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        // Danh sách sản phẩm đã phân trang
        public IPagedList<Product> Products { get; set; }
    }
}
