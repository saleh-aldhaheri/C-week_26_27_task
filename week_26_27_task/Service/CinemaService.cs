using System.Linq.Expressions;
using week_26_27.Repositories.IRepositories;
using week_26_27.Service.IService;

namespace week_26_27_task.Service;
public class CinemaService : ICinemaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPagination _pagination;
    private readonly IFileHelper _fileHelper;
    private readonly IAuditoriumService _auditoriumService;

    private readonly string _imageFilePath = "assets\\images\\cinema";

    public CinemaService(IUnitOfWork unitOfWork, IPagination pagination, IFileHelper fileHelper, IAuditoriumService auditoriumService)
    {
        _unitOfWork = unitOfWork;
        _pagination = pagination;
        _fileHelper = fileHelper;
        _auditoriumService = auditoriumService;
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

    public async Task CreateCinema(CinemaWithResources cinemaWithResources, CancellationToken ct = default)
    {
        var image = cinemaWithResources.Image;
        var cinema = cinemaWithResources.Cinema;

        var name = _fileHelper.GenerateName(image.FileName);
        var path = _fileHelper.GeneratePath(name, _imageFilePath);

        if (path is null || !_fileHelper.Upload(path, image))
            throw new Exception();

        cinema.Img = name;

        await _unitOfWork.cinemaRepository.Add(entity: cinema);

        await _unitOfWork.cinemaRepository.CommitAsync(ct);

        foreach (var auditorium in cinemaWithResources.CreateAuditorium)
            await _auditoriumService.CreateAuditorium(cinema.Id, auditorium, ct);
    }

    public async Task UpdateCinema(CinemaWithResources cinemaWithResource, CancellationToken ct = default)
    {
        var cinema = cinemaWithResource.Cinema;
        var image = cinemaWithResource.Image;

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
        var cinema = _unitOfWork.cinemaRepository.GetOne(exprission: e => e.Id == id, false, e => e.Auditoriums);

        if (cinema is null)
            throw new Exception();

        return cinema;
    }

    public IQueryable<Cinema> GetAllCinemas(Expression<Func<Cinema, bool>>? exprission = null)
    {
        return _unitOfWork.cinemaRepository.Get(exprission);
    }
}
