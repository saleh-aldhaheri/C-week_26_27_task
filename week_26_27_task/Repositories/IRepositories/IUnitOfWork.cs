using week_26_27.Models;
using week_26_27_task.Data.ApplicationDbContext;

namespace week_26_27.Repositories.IRepositories;
public interface IUnitOfWork : IDisposable
{
    IRepository<Category> categryRepository { get; }
    IRepository<Cinema> cinemaRepository { get; }
    IRepository<Movie> movieRepository { get; }
    IRepository<Actor> actorRepository { get; }
    IBulkRepository<MovieSubImg> movieSubImgRepository { get; }
    IBulkRepository<MovieActor> movieActorRepository { get; }
    IRepository<Auditorium> auditoriumRepository { get;}
    IBulkRepository<Seat> seatRepository { get; }
    IRepository<Booking> bookingRepository { get;}
    IRepository<Cart> cartRepository {get;}
    IRepository<CartSeat> cartSetRepository {get;}
    IRepository<Ticket> tickeetRepository {get;}
    ApplicationDbContext dbContext { get; }
}