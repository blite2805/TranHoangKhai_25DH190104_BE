using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    public partial class Product
    {
        [NotMapped]
        public HttpPostedFileBase UploadImg { get; set; }
    }
}