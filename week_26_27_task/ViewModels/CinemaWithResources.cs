using System.ComponentModel.DataAnnotations;
using week_26_27.ViewModels;

namespace week_26_27_task.ViewModels;
public class CinemaWithResources
{
    public Cinema Cinema { get; set; } = new();

    [Display(Name = "Cinema Image")]
    public IFormFile? Image { get; set; }

    public ICollection<AuditoriumVM> CreateAuditorium { get; set; } = new List<AuditoriumVM>();
}
