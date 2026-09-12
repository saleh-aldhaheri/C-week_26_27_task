using System.Linq.Expressions;
using week_26_27.Repositories.IRepositories;

namespace week_26_27_task.Service;
public class CinemaService : ICinemaService
{
    private IUnitOfWork _unitOfWork;
    private IPagination _pagination;

    private IFileHelper _fileHelper;

    private readonly string _imageFilePath = "assets\\images\\cinema";

    public CinemaService(IUnitOfWork unitOfWork, IPagination pagination, IFileHelper fileHelper)
    {
        _unitOfWork = unitOfWork;
        _pagination = pagination;
        _fileHelper = fileHelper;
    }

    public CinemaWithFilterAndPaginationVM GetCinemas(CinemaWithFilterAndPaginationVM cinemasIndex)
    {
        var cinemas = _unitOfWork.cinemaRepository.Get();

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

        await _unitOfWork.cinemaRepository.Add(entity: cinema);

        await _unitOfWork.cinemaRepository.CommitAsync(ct);
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

        _unitOfWork.cinemaRepository.Update(cinema);

        await _unitOfWork.cinemaRepository.CommitAsync(ct);
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

        _unitOfWork.cinemaRepository.Delete(cinema);

        await _unitOfWork.cinemaRepository.CommitAsync(ct);
    }

    public Cinema GetCinema(int id)
    {
        var cinema = _unitOfWork.cinemaRepository.GetOne(exprission: e => e.Id == id, false);

        if (cinema is null)
            throw new Exception();

        return cinema;
    }

    public IQueryable<Cinema> GetAllCinemas(Expression<Func<Cinema, bool>>? exprission = null)
    {
        return _unitOfWork.cinemaRepository.Get(exprission);
    }
}
