namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho partial _PersonalMenuPV.cshtml (góc trên phải layout khách hàng)
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; }
        public int CartCount { get; set; }
    }
}
