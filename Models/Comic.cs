namespace WebTruyen.Models
{
    public class Comic
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public string LatestChap { get; set; }
        public List<string> Chapters { get; set; }
    }
}
