using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string MainImg { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        [FutureDateTime]
        public DateTime StartAt { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; } 

        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }
        public ICollection<MovieSubImg> MovieSubImgs { get; set; } = new List<MovieSubImg>();
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
