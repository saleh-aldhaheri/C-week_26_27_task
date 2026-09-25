using week_26_27.Models;
using week_26_27.Repositories.IRepositories;

namespace week_26_27_task.Service;

public class MovieService : IMovieService
{
    private readonly IFileHelper _fileHelper;
    private readonly IPagination _pagination;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _imageFilePath = "assets\\images\\movie";
    private readonly string _subImageFilePath = "assets\\images\\movieSubImage";

    public MovieService(
        IFileHelper fileHelper,
        IPagination pagination,
        IUnitOfWork unitOfWork
    )
    {
        _fileHelper = fileHelper;
        _pagination = pagination;
        _unitOfWork = unitOfWork;
    }

    public MovieWithFilterAndPaginationVM GetMovies(MovieWithFilterAndPaginationVM moviesIndex)
    {
        var movies = _unitOfWork.movieRepository.Get(null,true, [e => e.Category,
                e => e.Auditorium, e => e.Auditorium.Cinema]);

        var categories = _unitOfWork.categryRepository.Get().Where(e => e.Status == true).Select(e => new Category
        {
            Id = e.Id,
            Name = e.Name
        });

        var cinemas = _unitOfWork.cinemaRepository.Get().Select(e => new Cinema
        {
            Id = e.Id,
            Name = e.Name
        });

        var auditoriums = _unitOfWork.auditoriumRepository.Get().Select(e => new Auditorium
        {
            Id = e.Id,
            Name = e.Name
        });

        if (moviesIndex.Search is not null)
            movies = movies.Where(e => e.Title.ToLower().Contains(moviesIndex.Search.ToLower()) ||
                e.Description.ToLower().Contains(moviesIndex.Search.ToLower()));

        if (moviesIndex.MinPrice is not null)
            movies = movies.Where(e => e.Price >= moviesIndex.MinPrice);

        if (moviesIndex.MaxPrice is not null)
            movies = movies.Where(e => e.Price <= moviesIndex.MaxPrice);

        if (moviesIndex.StartAt is not null)
            movies = movies.Where(e => e.StartAt.Date == moviesIndex.StartAt.Value.Date);

        if (moviesIndex.CategoriasId is not null)
            movies = movies.Where(e => e.CategoryId == moviesIndex.CategoriasId);

        if (moviesIndex.CinemaId is not null)
            movies = movies.Where(e => e.Auditorium.CinemaId == moviesIndex.CinemaId);

        if(moviesIndex.AuditoriumId is not null)
            movies = movies.Where(e => e.AuditoriumId == moviesIndex.AuditoriumId);
        
        int skip = (moviesIndex.Pagination.Page - 1) * moviesIndex.Pagination.PageSize;

        moviesIndex.Movies = movies.Skip(skip).Take(moviesIndex.Pagination.PageSize);

        moviesIndex.Pagination = _pagination
            .Paginate(
            movies.Count(),
            moviesIndex.Pagination.PageSize,
            moviesIndex.Pagination.Page
            );

        moviesIndex.Categorias = categories;
        moviesIndex.Cinemas = cinemas;
        moviesIndex.Auditoriums = auditoriums;

        return moviesIndex;
    }

    public async Task CreateMovie(MovieWithCategoriesCinemasActors movieWithResouce, CancellationToken ct = default)
    {
        var image = movieWithResouce.Image;
        var movie = movieWithResouce.Movie;
        var images = movieWithResouce.Images;
        var actorsIds = movieWithResouce.SelectedActorIds;

        string name = _fileHelper.GenerateName(image.FileName);
        string? path = _fileHelper.GeneratePath(name, _imageFilePath);

        if (path is null || !_fileHelper.Upload(path, image))
            throw new Exception();


        var auditorimu = _unitOfWork.auditoriumRepository.GetOne(e => e.Id == movieWithResouce.SelectedAuditoriumId); 
        var  cinema = _unitOfWork.cinemaRepository.GetOne( e => e.Id == movieWithResouce.SelectedCinemaId);
        var category = _unitOfWork.categryRepository.GetOne(e => e.Id == movieWithResouce.SelectedCategroyId);

        if (auditorimu is null || cinema is null || category is null)
            throw new Exception();

        if (!cinema.Auditoriums.Select(e => e.Id).Contains(auditorimu.Id))
            throw new Exception(); 

        movie.MainImg = name;
        movie.AuditoriumId = movieWithResouce.SelectedAuditoriumId;
        movie.CategoryId = movieWithResouce.SelectedCategroyId;

        await _unitOfWork.movieRepository.Add(movie);
        await _unitOfWork.movieRepository.CommitAsync();

        if (images is not null && images.Any())
        {
            await CreateSubImages(movie.Id, images,ct);
        }

        if (actorsIds is not null && actorsIds.Any())
        {
            await CreateMoiveActors(movie.Id, actorsIds, ct);
        }
    }

    public async Task UpdateMovie(MovieWithCategoriesCinemasActors movieWithResource, CancellationToken ct = default)
    {
        var movie = movieWithResource.Movie;
        var image = movieWithResource.Image;
        var images = movieWithResource.Images;
        var actorsIds = movieWithResource.SelectedActorIds;

        Movie DbMovie = GetMovie(movie.Id);

        var auditorimu = _unitOfWork.auditoriumRepository.GetOne(e => e.Id == movieWithResource.SelectedAuditoriumId);
        var cinema = _unitOfWork.cinemaRepository.GetOne(e => e.Id == movieWithResource.SelectedCinemaId);
        var category = _unitOfWork.categryRepository.GetOne(e => e.Id == movieWithResource.SelectedCategroyId);

        if (auditorimu is null || cinema is null || category is null)
            throw new Exception();

        movie.CategoryId = category.Id;

        if (!cinema.Auditoriums.Select(e => e.Id).Contains(auditorimu.Id))
            throw new Exception();

        movie.AuditoriumId = auditorimu.Id;

        if (image is not null)
        {
            string name = _fileHelper.GenerateName(image.FileName);
            string? path = _fileHelper.GeneratePath(name, _imageFilePath);

            if (path is null || !_fileHelper.Upload(path, image))
                throw new Exception();

            movie.MainImg = name;

            if(DbMovie.MainImg is not null)
            {
                var oldPath = _fileHelper.GeneratePath(DbMovie.MainImg, _imageFilePath);
                if(oldPath is not null)
                     _fileHelper.Delete(oldPath); 
            }
        }
        else
        {
            movie.MainImg = DbMovie.MainImg; 
        }

        _unitOfWork.movieRepository.Update(movie);
        await _unitOfWork.movieRepository.CommitAsync(ct);

        if(images is not null && images.Any())
        {
            await deleteSubImages(movie.Id, ct);
            await CreateSubImages(movie.Id,images, ct); 
        }

        await deleteMovieActors(movie.Id, ct);
        if (actorsIds is not null && actorsIds.Any())
        {
            await CreateMoiveActors(movie.Id, actorsIds, ct);
        }
    }

    public async Task DeleteMovie(int id, CancellationToken ct = default)
    {
        Movie movie = GetMovie(id);

        string? path = _fileHelper.GeneratePath(movie.MainImg, _imageFilePath);

        if (path is not null)
        {
            _fileHelper.Delete(path);
        }

        await deleteSubImages(id, ct);

        await deleteMovieActors(id, ct);

        _unitOfWork.movieRepository.Delete(movie);
        await _unitOfWork.movieRepository.CommitAsync(ct);
    }


    public (IQueryable<Cinema>, IQueryable<Category>, IQueryable<Actor>) GetDropDowns()
    {
        var cinemas = _unitOfWork.cinemaRepository.Get().Select(e => new Cinema
        {
            Id = e.Id,
            Name = e.Name,
            Auditoriums = e.Auditoriums.Select(a => new Auditorium
            {
                Id = a.Id,
                Name = a.Name
            }).ToList()
        });

        var categories = _unitOfWork.categryRepository.Get(e => e.Status == true).Select(e => new Category
        {
            Id = e.Id,
            Name = e.Name,
        });

        var actors = _unitOfWork.actorRepository.Get().Select(e => new Actor
        {
            Id = e.Id,
            FullName = e.FullName,
            Img = e.Img
        });

        return (cinemas, categories, actors);
    }

    //helper methods 
    public Movie GetMovie(int id)
    {
        Movie? movie = _unitOfWork.movieRepository.GetOne(
            e => e.Id == id, 
            false,
            relations: [
                e => e.MovieActors,
                e => e.MovieSubImgs,
                e => e.Category,
                e => e.Auditorium,
                e => e.Auditorium.Cinema
                ]);

        if (movie is null)
            throw new Exception();

        return movie;
    }

    private async Task CreateSubImages(int movieId, List<IFormFile> images, CancellationToken ct)
    {
        foreach (var img in images)
        {
            string imgName = _fileHelper.GenerateName(img.FileName);
            string? imgPath = _fileHelper.GeneratePath(imgName, _subImageFilePath);

            if (imgPath is not null)
            {
                _fileHelper.Upload(imgPath, img);

                await _unitOfWork.movieSubImgRepository.Add(new()
                {
                    MovieId = movieId,
                    Img = imgName
                });

            }
        }

        await _unitOfWork.movieSubImgRepository.CommitAsync();
    }

    private async Task CreateMoiveActors(int movieId, List<int> actorsIds, CancellationToken ct)
    {
        foreach (var id in actorsIds)
        {
            await _unitOfWork.movieActorRepository.Add(new()
            {
                MovieId = movieId,
                ActorId = id
            });
        }

        await _unitOfWork.movieActorRepository.CommitAsync();
    }

    private async Task deleteSubImages(int moiveId, CancellationToken ct)
    {
        var subImages = _unitOfWork.movieSubImgRepository.Get(e => e.MovieId == moiveId);

        foreach (var subImage in subImages)
        {
            var path = _fileHelper.GeneratePath(subImage.Img, _subImageFilePath);
            if (path is not null)
            {
                _fileHelper.Delete(path);
            }
        }

        _unitOfWork.movieSubImgRepository.DeleteRange(subImages);

        await _unitOfWork.movieSubImgRepository.CommitAsync(ct);

    }

    private async Task deleteMovieActors(int movieId, CancellationToken ct)
    {
        var movieActors = _unitOfWork.movieActorRepository.Get(e => e.MovieId == movieId);
        _unitOfWork.movieActorRepository.DeleteRange(movieActors);

        await _unitOfWork.movieActorRepository.CommitAsync();
    }
}
