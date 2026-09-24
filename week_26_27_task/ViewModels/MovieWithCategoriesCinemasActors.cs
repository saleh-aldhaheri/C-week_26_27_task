using System.ComponentModel.DataAnnotations;
using week_26_27.Models;

public class MovieWithCategoriesCinemasActors
{
    public Movie Movie { get; set; } = null!;
    public IEnumerable<Category>? Categories { get; set; }
    public IEnumerable<Cinema>? CinemasWithAuditoriums { get; set; }
    public IEnumerable<Actor>? Actors { get; set; }
    public List<int>? SelectedActorIds { get; set; }
    
    [Display( Name = "Cinema")]
    [Required]
    public int SelectedCinemaId { get; set; } = 0;

    [Display(Name = "Auditorium")]
    [Required]
    public int SelectedAuditoriumId { get; set; } = 0;

    [Display(Name = "Category")]
    [Required]
    public int SelectedCategroyId { get; set; } = 0; 

    [Display(Name = "Movie Image")]
    public IFormFile Image { get; set; } = null!;

    [Display(Name = "Movie Images")]
    public List<IFormFile>? Images { get; set; }
}
