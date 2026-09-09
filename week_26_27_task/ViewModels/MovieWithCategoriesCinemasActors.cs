using System.ComponentModel.DataAnnotations;

public class MovieWithCategoriesCinemasActors
{
    public Movie Movie { get; set; } = null!;
    public IEnumerable<Category>? Categories { get; set; }
    public IEnumerable<Cinema>? Cinemas { get; set; }
    public IEnumerable<Actor>? Actors { get; set; }
    public List<int>? SelectedActorIds { get; set; } 

    [Display(Name = "Movie Image")]
    public IFormFile? Image { get; set; }

    [Display(Name = "Movie Images")]
    public List<IFormFile>? Images { get; set; }
}
