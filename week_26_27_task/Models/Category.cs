using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [LetterOnly]
        public string Name { get; set; } = null!;

        [StringLength(1000, MinimumLength = 0)]
        public string? Description { get; set; }

        [Required]
        public bool Status { get; set; }
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
