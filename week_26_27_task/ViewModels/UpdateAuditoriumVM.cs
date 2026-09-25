using System.ComponentModel.DataAnnotations;

namespace week_26_27_task.ViewModels;

public class UpdateAuditoriumVM
{
    public int Id { get; set; }
    
    public int CinemaId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [Display(Name = "Auditorium name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, 20, ErrorMessage = "Rows must be between 1 and 20.")]
    [Display(Name = "Rows")]
    public int Rows { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Columns must be between 1 and 50.")]
    [Display(Name = "Columns")]
    
    public int Columns { get; set; }
}
