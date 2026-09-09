using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace week_26_27_task.ViewModels;
public class CinemaWithImageVm
{
    public Cinema Cinema { get; set; } = new();

    [Display(Name = "Cinema Image")]
    public IFormFile? Image { get; set; }
}
