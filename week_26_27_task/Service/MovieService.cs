using Microsoft.EntityFrameworkCore;

namespace week_26_27_task.Service;

public class MovieService : IMovieService
{
    private readonly IFileHelper _fileHelper;
    private readonly IPagination _pagination;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Cinema> _cinemaRepository;
    private readonly IRepository<Actor> _actorRepository;
    private readonly IBulkRepository<MovieSubImg> _movieSubImgBulkRepository;
    private readonly IBulkRepository<MovieActor> _movieActorBulkRespository;
    private readonly string _imageFilePath = "assets\\images\\movie";
    private readonly string _subImageFilePath = "assets\\images\\movieSubImage";

    public MovieService(
        IFileHelper fileHelper,
        IPagination pagination,
        IRepository<Movie> movieRepository,
        IRepository<Category> categoryRepository,
        IRepository<Cinema> cinemaRepository,
        IRepository<Actor> actorRepositry,
        IBulkRepository<MovieActor> movieActorBulkRespository,
        IBulkRepository<MovieSubImg> movieSubImgBulkRepository
    )
    {
        _fileHelper = fileHelper;
        _pagination = pagination;
        _movieRepository = movieRepository;
        _categoryRepository = categoryRepository;
        _cinemaRepository = cinemaRepository;
        _actorRepository = actorRepositry;
        _movieActorBulkRespository = movieActorBulkRespository;
        _movieSubImgBulkRepository = movieSubImgBulkRepository;
    }

    public MovieWithFilterAndPaginationVM GetMovies(MovieWithFilterAndPaginationVM moviesIndex)
    {
        var movies = _movieRepository.Get(null,true, [e => e.Category,
                e => e.Cinema]);

        var categories = _categoryRepository.Get().Where(e => e.Status == true).Select(e => new Category
        {
            Id = e.Id,
            Name = e.Name
        });

        var cinemas = _cinemaRepository.Get().Select(e => new Cinema
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
            movies.Where(e => e.StartAt == moviesIndex.StartAt);

        if (moviesIndex.CategoriasId is not null)
            movies = movies.Where(e => e.CategoryId == moviesIndex.CategoriasId);

        if (moviesIndex.CinemaId is not null)
            movies = movies.Where(e => e.CinemaId == moviesIndex.CinemaId);

        int skip = (moviesIndex.Pagination.Page - 1) * moviesIndex.Pagination.PageSize;

        moviesIndex.Movies = movies.Skip(skip).Take(moviesIndex.Pagination.PageSize);

        moviesIndex.Pagination = _pagination
            .Paginate(
            cinemas.Count(),
            moviesIndex.Pagination.PageSize,
            moviesIndex.Pagination.Page
            );

        moviesIndex.Categorias = categories;
        moviesIndex.Cinemas = cinemas;

        return moviesIndex;
    }

    public async Task CreateMovie(Movie movie, IFormFile image, List<IFormFile>? images, List<int>? actorsIds, CancellationToken ct = default)
    {
        string name = _fileHelper.GenerateName(image.FileName);
        string? path = _fileHelper.GeneratePath(name, _imageFilePath);

        if (path is null || !_fileHelper.Upload(path, image))
            throw new Exception();

        movie.MainImg = name;

        await _movieRepository.Add(movie);
        await _movieRepository.CommitAsync();

        if (images is not null && images.Any())
        {
            await CreateSubImages(movie.Id, images,ct);
        }

        if (actorsIds is not null && actorsIds.Any())
        {
            await CreateMoiveActors(movie.Id, actorsIds, ct);
        }
    }

    public async Task UpdateMovie(Movie movie, IFormFile? image, List<IFormFile>? images, List<int>? actorsIds, CancellationToken ct = default)
    {
        Movie DbMovie = GetMovie(movie.Id); 

        if(image is not null)
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

        _movieRepository.Update(movie);

        if(images is not null && images.Any())
        {
            await deleteSubImages(movie.Id, ct);
            await CreateSubImages(movie.Id,images, ct); 
        }

        if (actorsIds is not null && actorsIds.Any())
        {
            await deleteMovieActors(movie.Id, ct);
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

        _movieRepository.Delete(movie);
        await _movieRepository.CommitAsync(ct);
    }


    public (IQueryable<Cinema>, IQueryable<Category>, IQueryable<Actor>) GetDropDowns()
    {
        var cinemas = _cinemaRepository.Get().Select(e => new Cinema
        {
            Id = e.Id,
            Name = e.Name
        });

        var categories = _categoryRepository.Get(e => e.Status == true).Select(e => new Category
        {
            Id = e.Id,
            Name = e.Name,
        });

        var actors = _actorRepository.Get().Select(e => new Actor
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
        Movie? movie = _movieRepository.GetOne(
            e => e.Id == id, 
            false,
            relations: [
                e => e.MovieActors,
                e => e.MovieSubImgs,
                e => e.Category,
                e => e.Cinema
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

                await _movieSubImgBulkRepository.Add(new()
                {
                    MovieId = movieId,
                    Img = imgName
                });

            }
        }

        await _movieSubImgBulkRepository.CommitAsync();
    }

    private async Task CreateMoiveActors(int movieId, List<int> actorsIds, CancellationToken ct)
    {
        foreach (var id in actorsIds)
        {
            await _movieActorBulkRespository.Add(new()
            {
                MovieId = movieId,
                ActorId = id
            });
        }

        await _movieActorBulkRespository.CommitAsync();
    }

    private async Task deleteSubImages(int moiveId, CancellationToken ct)
    {
        var subImages = _movieSubImgBulkRepository.Get(e => e.MovieId == moiveId);

        foreach (var subImage in subImages)
        {
            var path = _fileHelper.GeneratePath(subImage.Img, _subImageFilePath);
            if (path is not null)
            {
                _fileHelper.Delete(path);
            }
        }

        _movieSubImgBulkRepository.DeleteRange(subImages);

        await _movieActorBulkRespository.CommitAsync(ct);

    }

    private async Task deleteMovieActors(int movieId, CancellationToken ct)
    {
        var movieActors = _movieActorBulkRespository.Get(e => e.MovieId == movieId);
        _movieActorBulkRespository.DeleteRange(movieActors);

        await _movieActorBulkRespository.CommitAsync();
    }
}
