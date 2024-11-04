using Microsoft.AspNetCore.Mvc;
using WebTruyen.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlClient;

namespace WebTruyen.Controllers
{
    public class ComicsController : Controller
    {
        private readonly string _comicsDirectory;
        private readonly string _connectionString;
        public ComicsController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _comicsDirectory = Path.Combine(env.WebRootPath, "comic");
        }

        public IActionResult Index()
        {
            var comics = new List<Comic>();

            if (Directory.Exists(_comicsDirectory))
            {
                var comicFolders = Directory.GetDirectories(_comicsDirectory);

                foreach (var folder in comicFolders)
                {
                    var title = Path.GetFileName(folder);
                    var chapterFolders = Directory.GetDirectories(folder);

                    if (chapterFolders.Length > 0)
                    {
                        var latestChapter = Path.GetFileName(chapterFolders.Last());
                        var coverPath = $"/comic/{title}/0.jpg";

                        comics.Add(new Comic
                        {
                            Title = title,
                            Cover = coverPath,
                            LatestChap = latestChapter
                        });
                    }
                }
            }

            return View(comics);
        }

        public IActionResult Detail(string title)
        {
            // Đường dẫn tới folder truyện cụ thể
            var comicPath = Path.Combine(_comicsDirectory, title);

            if (!Directory.Exists(comicPath))
            {
                return NotFound();
            }

            var chapterFolders = Directory.GetDirectories(comicPath).Select(Path.GetFileName).OrderBy(chap => chap).ToList();
            var comic = new Comic
            {
                Title = title,
                Cover = $"/comic/{title}/0.jpg",
                LatestChap = chapterFolders.LastOrDefault(),
                Chapters = chapterFolders
            };

            return View(comic);
        }

        public IActionResult Chapter(string title, string chapter)
        {
            var comicPath = Path.Combine(_comicsDirectory, title);
            var chapterPath = Path.Combine(comicPath, chapter);

            if (!Directory.Exists(chapterPath))
            {
                return NotFound();
            }

            var imageFiles = Directory.GetFiles(chapterPath, "*.jpg").OrderBy(f => f).Select(Path.GetFileName).ToList();

            // Chỉnh sửa ở đây
            int currentChapterIndex;
            if (chapter.StartsWith("chap", StringComparison.OrdinalIgnoreCase))
            {
                currentChapterIndex = int.Parse(chapter.Replace("chap", ""));
            }
            else
            {
                return NotFound(); // Trường hợp không hợp lệ
            }

            var comicModel = new
            {
                Title = title,
                Chapter = chapter,
                Images = imageFiles,
                CurrentChapterIndex = currentChapterIndex // Sử dụng biến đã điều chỉnh
            };

            return View(comicModel);
        }
        [HttpGet]
        public IActionResult Search(string query)
        {
            var comics = new List<Comic>();
            var cmcs = new List<string>();


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT TENTRUYEN FROM TRUYEN WHERE TENTRUYEN LIKE %@tentruyen%", connection))
                {
                    command.Parameters.AddWithValue("@tentruyen", query);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmcs.Add(reader.GetString(0));
                        }
                        connection.Close();
                    }
                }
            }
            foreach (var item in cmcs)
            {
                var path = Path.Combine(_comicsDirectory, item);
                if (!Directory.Exists(path))
                {

                }
                else
                {
                    Comic comic = new Comic
                    {
                        Title = item,
                        Cover = $"/comic/{item}/0.jpg",

                    };
                    comics.Add(comic);
                }

              
            }

            return View("_SearchResults", comics);

        }
    }
}
