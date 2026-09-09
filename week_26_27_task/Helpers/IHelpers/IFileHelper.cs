namespace week_26_27_task.Helpers.IHelpers; 

public interface IFileHelper
{
    public string GenerateName(string fileName);
    public string? GeneratePath (string name, string entityFileName);
    public bool Upload(string path, IFormFile file);
    public bool Delete(string path);
}
