using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [LetterOnly]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [StringLength(1000, MinimumLength =0)]
        public string? Bio { get; set; }

        public string Img { get; set; } = string.Empty;

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
