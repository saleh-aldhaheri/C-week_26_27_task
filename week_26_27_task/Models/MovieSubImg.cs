namespace week_26_27_task.Models
{
    public class MovieSubImg
    {
        public int Id { get; set; }
        public string Img { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
