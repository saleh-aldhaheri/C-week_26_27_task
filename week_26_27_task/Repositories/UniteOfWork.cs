using week_26_27.Models;
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
            IBulkRepository<MovieActor> movieActorRepository,
            IRepository<Auditorium> auditoriumRepository,
            IBulkRepository<Seat> seatRepository,
            IRepository<Booking> bookingRepository,
            IRepository<Cart> cartRepository,
            IRepository<CartSeat> cartSetRepository,
            IRepository<Ticket> tickeetRepository,
            ApplicationDbContext dbContext
        )
        {
            this.categryRepository = categryRepository;
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.movieSubImgRepository = movieSubImgRepository;
            this.movieActorRepository = movieActorRepository;
            this.auditoriumRepository = auditoriumRepository;
            this.seatRepository = seatRepository;
            this.bookingRepository = bookingRepository;
            this.tickeetRepository = tickeetRepository;
            this.cartRepository = cartRepository;
            this.cartSetRepository = cartSetRepository;
            this.dbContext = dbContext;
        }

        public IRepository<Category> categryRepository { get; }
        public IRepository<Cinema> cinemaRepository { get; }
        public IRepository<Movie> movieRepository { get; }
        public IRepository<Actor> actorRepository { get; }
        public IRepository<Auditorium> auditoriumRepository { get; }
        public IBulkRepository<Seat> seatRepository { get; }
        public IRepository<Booking> bookingRepository { get; }
        public IRepository<Cart> cartRepository { get; }
        public IRepository<CartSeat> cartSetRepository { get; }
        public IRepository<Ticket> tickeetRepository { get; }
        public IBulkRepository<MovieSubImg> movieSubImgRepository { get; }
        public IBulkRepository<MovieActor> movieActorRepository { get; }

        public ApplicationDbContext dbContext { get; }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}