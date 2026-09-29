using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using week_26_27.Repositories.IRepositories;
using week_26_27.ViewModels;

namespace week_26_27.Areas.Customer.Controllers;

[Area(AreaConstants.CUSTOMER_AREA)]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPagination _pagination;

    public HomeController(IUnitOfWork unitOfWork, IPagination pagination)
    {
        _unitOfWork = unitOfWork;
        _pagination = pagination;
    }

    public IActionResult Index(MovieWithFilterAndPaginationVM movieWithFilterAndPaginationVm)
    {
        var movies = _unitOfWork.movieRepository.Get(e => e.StartAt > DateTime.Now, false)
            .Include(e => e.Category)
            .Include(e => e.MovieSubImgs)
            .Include(e => e.MovieActors)
            .ThenInclude(m => m.Actor)
            .Include(e => e.Auditorium)
            .ThenInclude(a => a.Cinema)
            .AsQueryable();

        if(movieWithFilterAndPaginationVm.CinemaId is not null)
        {
            movies = movies.Where(m => m.Auditorium.CinemaId == movieWithFilterAndPaginationVm.CinemaId);
        }

        if (movieWithFilterAndPaginationVm.CategoriasId is not null)
        {
            movies = movies.Where(m => m.CategoryId == movieWithFilterAndPaginationVm.CategoriasId);
        }

        if(movieWithFilterAndPaginationVm.ActorId is not null)
        {
            movies = movies.Where(e => e.MovieActors.Any( m => m.ActorId == movieWithFilterAndPaginationVm.ActorId));
        }

        if(movieWithFilterAndPaginationVm.MinPrice is not null)
        {
            movies = movies.Where(e => e.Price >= movieWithFilterAndPaginationVm.MinPrice);
        }

        if(movieWithFilterAndPaginationVm.MaxPrice is not null)
        {
            movies = movies.Where(e => e.Price <= movieWithFilterAndPaginationVm.MaxPrice);
        }

        if(movieWithFilterAndPaginationVm.Search is not null)
        {
            var searchable = movieWithFilterAndPaginationVm.Search.ToLower();
            movies = movies.Where(
                e => e.Title.ToLower().Contains(searchable) ||
                    e.Category.Name.ToLower().Contains(searchable) ||
                    e.Auditorium.Cinema.Name.ToLower().Contains(searchable) ||
                    e.MovieActors.Any(e => e.Actor.FullName.Contains(searchable))
            );
        }

        if(movieWithFilterAndPaginationVm.StartAt is not null)
        {
            movies = movies.Where(e => e.StartAt == movieWithFilterAndPaginationVm.StartAt);
        }

        var size = movieWithFilterAndPaginationVm.Pagination.PageSize == 15 ? 6 : movieWithFilterAndPaginationVm.Pagination.PageSize;
        var pagination = _pagination.Paginate(movies.Count(), size, movieWithFilterAndPaginationVm.Pagination.Page);

        movieWithFilterAndPaginationVm.Auditoriums = _unitOfWork.auditoriumRepository.Get();
        movieWithFilterAndPaginationVm.Categorias = _unitOfWork.categryRepository.Get();
        movieWithFilterAndPaginationVm.Actors = _unitOfWork.actorRepository.Get();
        movieWithFilterAndPaginationVm.Cinemas = _unitOfWork.cinemaRepository.Get();

        int skip = (movieWithFilterAndPaginationVm.Pagination.Page - 1) * movieWithFilterAndPaginationVm.Pagination.PageSize;

        movieWithFilterAndPaginationVm.Movies = movies.Skip(skip).Take(movieWithFilterAndPaginationVm.Pagination.PageSize);
        movieWithFilterAndPaginationVm.Pagination = pagination;
        

        return View(movieWithFilterAndPaginationVm);
    }

    public IActionResult Show(int id)
    {
        var movie = _unitOfWork.movieRepository.Get(e => e.Id == id, false)
            .Include(e => e.Category)
            .Include(e => e.MovieSubImgs)
            .Include(e => e.MovieActors)
            .ThenInclude(m => m.Actor)
            .FirstOrDefault();

        if (movie is null)
            return NotFound();

        var tickets =  _unitOfWork.tickeetRepository
            .Get(e => e.Booking.MovieId == movie.Id);

        var ticketSeatIds = tickets.Select(t => t.SeatId).ToHashSet();

        var cartSeats =  _unitOfWork.cartSetRepository
            .Get(e => e.Cart.MovieId == movie.Id && e.ExpiredAt > DateTime.Now);

        var unavailableSeatIds = ticketSeatIds
            .Union(cartSeats.Select(c => c.SeatId))
            .ToHashSet();

        var auditorium =  _unitOfWork.auditoriumRepository.GetOne(
             e => e.Id == movie.AuditoriumId,
         false,
             e => e.Cinema, e => e.Seats
        );

        if(auditorium is not null)
        {
            auditorium.Seats = auditorium.Seats.Where(e => !unavailableSeatIds.Contains(e.Id)).ToList();
        }else
        {
            return NotFound();
        }
        
        return View(new CustomerMovieShowVM
        {
            Movie = movie,
            Auditorium = auditorium
        });
    }
}
