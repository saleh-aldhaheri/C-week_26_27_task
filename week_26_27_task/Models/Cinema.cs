using System.ComponentModel.DataAnnotations;
using week_26_27.Models;

namespace week_26_27_task.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength =3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 3)]
        public string Location { get; set; } = string.Empty;

        public string Img { get; set; } = string.Empty;

        public ICollection<Auditorium> Auditoriums { get; set; } = new List<Auditorium>();
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
