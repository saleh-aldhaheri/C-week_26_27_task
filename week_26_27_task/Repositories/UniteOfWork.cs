using week_26_27.Repositories.IRepositories;
using week_26_27_task.Data.ApplicationDbContext;

namespace week_26_27.Repositories
{
    public class UniteOfWork : IUnitOfWork
    {
        public UniteOfWork(
            IRepository<Category> categryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Movie> movieRepository,
            IRepository<Actor> actorRepository,
            IBulkRepository<MovieSubImg> movieSubImgRepository,
            IBulkRepository<MovieActor> movieActorRespository,
            ApplicationDbContext dbContext
        )
        {
            this.categryRepository = categryRepository;
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.movieSubImgRepository = movieSubImgRepository;
            this.movieActorRespository = movieActorRespository;
            this.dbContext = dbContext;
        }

        public IRepository<Category> categryRepository { get; }
        
        public IRepository<Cinema> cinemaRepository { get; }

        public IRepository<Movie> movieRepository { get; }

        public IRepository<Actor> actorRepository { get; }

        public IBulkRepository<MovieSubImg> movieSubImgRepository { get; }

        public IBulkRepository<MovieActor> movieActorRespository { get; }

        public ApplicationDbContext dbContext { get; }

        public void Dispose()
        {
            dbContext.Dispose(); 
        }
    }
}