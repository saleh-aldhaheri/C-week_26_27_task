using week_26_27.Models;
using week_26_27.Repositories.IRepositories;
using week_26_27.Service.IService;
using week_26_27.ViewModels;

namespace week_26_27.Service;

public class AuditoriumService : IAuditoriumService
{
    private readonly ISeatService _seatService;
    private readonly IUnitOfWork _unitOfWork;

    public AuditoriumService(IUnitOfWork unitOfWork, ISeatService seatService)
    {
        _seatService = seatService;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAuditorium(int cinemaId, AuditoriumVM createAuditorium, CancellationToken ct)
    {
        var auditorum = new Auditorium()
        {
            Name = createAuditorium.Name,
            CinemaId = cinemaId,
            Rows = createAuditorium.Rows,
            Columns = createAuditorium.Columns
        };

        await _unitOfWork.auditoriumRepository.Add(auditorum);
        await _unitOfWork.auditoriumRepository.CommitAsync(ct);

        await _seatService.CreateSeats(auditorum.Id, createAuditorium.Rows, createAuditorium.Columns, ct);
    }

    public async Task<Dictionary<string, string>?> UpdateAuditorium(UpdateAuditoriumVM updateAuditoriumVm, CancellationToken ct)
    {
        var auditorium = _unitOfWork.auditoriumRepository.GetOne(e => e.Id == updateAuditoriumVm.Id, true)
            ?? throw new Exception();

        var seatsChanged = auditorium.Rows != updateAuditoriumVm.Rows ||
                           auditorium.Columns != updateAuditoriumVm.Columns;

        if (seatsChanged && HasActiveBookings(auditorium.Id))
        {
            return new Dictionary<string, string>
            {
                { NotificationConstants.ERROR_NOTIFICATION, "You can't change the seats while this auditorium has active bookings." }
            };
        }

        auditorium.Name = updateAuditoriumVm.Name;
        auditorium.Rows = updateAuditoriumVm.Rows;
        auditorium.Columns = updateAuditoriumVm.Columns;

        _unitOfWork.auditoriumRepository.Update(auditorium);
        await _unitOfWork.auditoriumRepository.CommitAsync(ct);

        if (seatsChanged)
            await _seatService.ResetSeats(auditorium.Id, auditorium.Rows, auditorium.Columns, ct);

        return null;
    }

    public async Task<Dictionary<string,string>?> DeleteAuditorium(int auditoriumId, CancellationToken ct)
    {
        var auditorium = _unitOfWork.auditoriumRepository.GetOne(e => e.Id == auditoriumId, true)
            ?? throw new Exception();

        if (HasActiveBookings(auditorium.Id))
        {
            return new Dictionary<string, string>
            {
                { NotificationConstants.ERROR_NOTIFICATION, "You can't delete an auditorium with active bookings. Cancel the bookings first." }
            };
        }

        _unitOfWork.auditoriumRepository.Delete(auditorium);
        await _unitOfWork.auditoriumRepository.CommitAsync(ct);

        return null;
    }

    private bool HasActiveBookings(int auditoriumId)
    {
        var movieIds = _unitOfWork.movieRepository
            .Get(e => e.AuditoriumId == auditoriumId && e.StartAt > DateTime.Now, false)
            .Select(e => e.Id)
            .ToList();

        if (!movieIds.Any())
            return false;

        return _unitOfWork.bookingRepository
            .Get(e => movieIds.Contains(e.MovieId) && e.BookingStatus != BookingStatus.Canceled, false)
            .Any();
    }

    public Auditorium? GetAuditorim(int id)
    {
        return _unitOfWork.auditoriumRepository.GetOne(
            e => e.Id == id,
            false
        );
    }
}
