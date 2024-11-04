using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebTruyen.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebTruyen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        public static User _user = new User();

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }


        [HttpPost]
        public IActionResult DangNhap(LoginViewModel login)
        {
            if(ModelState.IsValid)
            {
                if(CheckLogin(login.UserName, login.Password))
                {

                    LoginViewModel.Logged = true;
                    return RedirectToAction("Index", "Comics");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"adsasdasd");
                }
            }
            
            return View();

        }

        [HttpGet]
        public IActionResult User()
        {
            return View(_user);
        }

        [HttpPost]
        public IActionResult ChangeUserInfo(User user)
        {
            var cnn = new SqlConnection(_connectionString);
            var query = "UPDATE NGUOIDUNG SET Email = @email, SDT = @sdt, Password= @password WHERE USERNAME = @username";
            cnn.Open();
            var cmd = new SqlCommand(query,cnn);
            cmd.Parameters.AddWithValue("email",user.Email);
            cmd.Parameters.AddWithValue("sdt",user.PhoneNumber);
            cmd.Parameters.AddWithValue("password",user.Password);
            cmd.Parameters.AddWithValue("username",_user.UserName);

            cmd.ExecuteNonQuery();
            cnn.Close();
            return RedirectToAction("User","Home",_user);
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {

                if(!register.Email.Contains("@") || !register.Email.Contains(".com"))
                {
                    ModelState.AddModelError(string.Empty, "Email không hợp lệ");
                    return View(register);
                }
                if(register.PhoneNumber.Length != 10)
                {
                    ModelState.AddModelError(string.Empty, "Số điện thoại không hợp lệ");
                    return View(register);
                }
                if(CheckLogin(register.UserName, register.Password))
                {
                    ModelState.AddModelError(string.Empty, "Người dùng đã tồn tại");
                    return View(register);
                }
                else
                {
                    var cnn = new SqlConnection(_connectionString);
                    cnn.Open();
                    var query = "INSERT INTO NGUOIDUNG (USERNAME, SDT, Email, Password) VALUES (@username, @phone, @email, @password)";
                    var cmd = new SqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("username", register.UserName);
                    cmd.Parameters.AddWithValue("phone", register.PhoneNumber);
                    cmd.Parameters.AddWithValue("email",register.Email);
                    cmd.Parameters.AddWithValue("password",register.Password);
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    return RedirectToAction("DangNhap", "Home");
                }
                
                
            }
            return View(register);
        }


        private bool CheckLogin(string username, string password)
        {
            var cnn = new SqlConnection(_connectionString);
            cnn.Open();
            var query = "SELECT * FROM NGUOIDUNG WHERE USERNAME = @username AND Password = @password";

            var cmd = new SqlCommand(query, cnn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);
            var result = cmd.ExecuteReader();
            if (result.Read())
            {
                _user.UserName = result[1].ToString();
                _user.Email = result[3].ToString();
                _user.Password = result[4].ToString();
                _user.PhoneNumber = result[2].ToString();
                cnn.Close();
                return true;
            }
            return false;

        } 

        public IActionResult DangXuat()
        {
            LoginViewModel.Logged = false;
            return RedirectToAction("Index","Comics");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
