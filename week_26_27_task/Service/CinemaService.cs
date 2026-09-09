using System.Linq.Expressions;

namespace week_26_27_task.Service;
public class CinemaService : ICinemaService
{
    private IRepository<Cinema> _repository;
    private IPagination _pagination;

    private IFileHelper _fileHelper;

    private readonly string _imageFilePath = "assets\\images\\cinema";

    public CinemaService(IRepository<Cinema> repository, IPagination pagination, IFileHelper fileHelper)
    {
        _repository = repository;
        _pagination = pagination;
        _fileHelper = fileHelper;
    }

    public CinemaWithFilterAndPaginationVM GetCinemas(CinemaWithFilterAndPaginationVM cinemasIndex)
    {
        var cinemas = _repository.Get();

        if (cinemasIndex.Search is not null)
        {
            cinemas = cinemas.Where(
                e => e.Name.ToLower().Contains(cinemasIndex.Search.ToLower()) ||
                         e.Location.ToLower().Contains(cinemasIndex.Search.ToLower())
                );
        }

        int skip = (cinemasIndex.Pagination.Page - 1) * cinemasIndex.Pagination.PageSize;

        cinemasIndex.Cinemas = cinemas.Skip(skip).Take(cinemasIndex.Pagination.PageSize);

        cinemasIndex.Pagination = _pagination
            .Paginate(
            cinemas.Count(),
            cinemasIndex.Pagination.PageSize,
            cinemasIndex.Pagination.Page
            );

        return cinemasIndex;
    }

    public async Task CreateCinema(Cinema cinema, IFormFile image, CancellationToken ct = default)
    {
        var name = _fileHelper.GenerateName(image.FileName);
        var path = _fileHelper.GeneratePath(name, _imageFilePath);

        if (path is null || !_fileHelper.Upload(path, image))
            throw new Exception();

        cinema.Img = name;

        await _repository.Add(entity: cinema);

        await _repository.CommitAsync(ct);
    }

    public async Task UpdateCinema(Cinema cinema, CancellationToken ct = default, IFormFile? image = null)
    {
        Cinema dbCinema = GetCinema(cinema.Id);

        if (image is not null)
        {
            var name = _fileHelper.GenerateName(image.FileName);
            var path = _fileHelper.GeneratePath(name, _imageFilePath);

            if (path is null || !_fileHelper.Upload(path, image))
                throw new Exception();

            cinema.Img = name;

            if (!string.IsNullOrEmpty(dbCinema.Img))
            {
                var oldPath = _fileHelper.GeneratePath(dbCinema.Img, _imageFilePath);
                if (oldPath is not null)
                    _fileHelper.Delete(oldPath);
            }
        }
        else
        {
            cinema.Img = dbCinema.Img;
        }

        _repository.Update(cinema);

        await _repository.CommitAsync(ct);
    }

    public async Task DeleteCinema(int id, CancellationToken ct = default)
    {
        Cinema cinema = GetCinema(id);

        if (!string.IsNullOrEmpty(cinema.Img))
        {
            var path = _fileHelper.GeneratePath(cinema.Img, _imageFilePath);

            if (path is not null)
                _fileHelper.Delete(path);
        }

        _repository.Delete(cinema);

        await _repository.CommitAsync(ct);
    }

    public Cinema GetCinema(int id)
    {
        var cinema = _repository.GetOne(exprission: e => e.Id == id, false);

        if (cinema is null)
            throw new Exception();

        return cinema;
    }

    public IQueryable<Cinema> GetAllCinemas(Expression<Func<Cinema, bool>>? exprission = null)
    {
        return _repository.Get(exprission);
    }
}
