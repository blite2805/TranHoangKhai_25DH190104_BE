using System.Collections.Generic;
using System.Web.SessionState;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // Service tĩnh quản lý giỏ hàng: lưu trong Session cho khách vãng lai,
    // và dictionary static UserCarts theo username để KHÔI PHỤC giỏ khi login/logout.
    // CartController giữ 2 method static SyncUserCart/GetUserCart delegate sang class này.
    public static class CartService
    {
        private const string CartSessionKey = "Cart";

        // Dictionary static: key = username, value = danh sách CartItem.
        // Giữ nguyên cơ chế khôi phục giỏ: khi logout SyncUserCart lưu lại, khi login GetUserCart khôi phục vào Session.
        // Public để controller checkout có thể UserCarts.Remove(username) sau khi đặt hàng thành công.
        public static Dictionary<string, List<CartItem>> UserCarts = new Dictionary<string, List<CartItem>>();

        // Lấy giỏ từ Session (tạo mới nếu chưa có)
        public static List<CartItem> GetCart(HttpSessionStateBase session)
        {
            if (session == null) return new List<CartItem>();
            if (session[CartSessionKey] == null)
            {
                session[CartSessionKey] = new List<CartItem>();
            }
            return (List<CartItem>)session[CartSessionKey];
        }

        // Lưu giỏ vào Session
        public static void SaveCart(HttpSessionStateBase session, List<CartItem> cart)
        {
            if (session == null) return;
            session[CartSessionKey] = cart;
        }

        // Đồng bộ giỏ vào dictionary theo username (gọi khi logout / thêm vào giỏ khi đã login)
        public static void SyncUserCart(string username, List<CartItem> cart)
        {
            if (string.IsNullOrEmpty(username)) return;
            UserCarts[username] = cart ?? new List<CartItem>();
        }

        // Lấy giỏ đã lưu theo username (gọi khi login để khôi phục giỏ cũ)
        public static List<CartItem> GetUserCart(string username)
        {
            if (string.IsNullOrEmpty(username)) return new List<CartItem>();
            if (UserCarts.ContainsKey(username))
            {
                return UserCarts[username] ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        // Xóa giỏ trong Session
        public static void ClearCart(HttpSessionStateBase session)
        {
            if (session == null) return;
            session[CartSessionKey] = null;
        }
    }
}
