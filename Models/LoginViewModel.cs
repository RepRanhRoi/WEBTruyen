using System.ComponentModel.DataAnnotations;
namespace WebTruyen.Models
{
    public class LoginViewModel
    {
        static public bool Logged = false;
        [Required(ErrorMessage ="Tên đăng nhập là bất buộc!")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Mật khẩu là bắt buộc!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}
