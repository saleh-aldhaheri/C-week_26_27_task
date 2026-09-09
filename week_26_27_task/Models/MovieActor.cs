namespace week_26_27_task.Models
{
    public class MovieActor
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;
    }
}
