namespace week_26_27_task.Helpers;
public class LocalFileHelper : IFileHelper
{
    public string GenerateName(string fileName)
    {
        return $"{Guid.NewGuid()}-{fileName}-{DateTime.Now.ToString("dd-MM-yyyy")}-{Path.GetExtension(fileName)}";
    }

    public string? GeneratePath(string name, string entityFileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entityFileName, name);
    }

    public bool Upload(string path, IFormFile file)
    {
        using (var stream = File.Create(path))
        {
            file.CopyTo(stream);
            return true;
        }
    }

    public bool Delete(string path)
    {
        if(File.Exists(path))
        {
            File.Delete(path);
            return true;
        }

        return false;
    }
}
