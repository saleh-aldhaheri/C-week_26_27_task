using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace week_26_27_task.ViewModels;
public class ActorWithImageVm
{
    public Actor Actor { get; set; } = new();

    [Display(Name = "Actor Image")]
    public IFormFile? Image { get; set; }
}
