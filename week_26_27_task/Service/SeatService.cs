using week_26_27.Models;
using week_26_27.Repositories.IRepositories;
using week_26_27.Service.IService;

namespace week_26_27.Service;

public class SeatService : ISeatService
{
    public IUnitOfWork _unitOfWork;

    public SeatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateSeats(int auditoriumId, int rows, int columns, CancellationToken ct)
    {
        var entities = new List<Seat>();

        for (int r = 1; r <= rows; r++)
        {

            for (int c = 1; c <= columns; c++)
            {
                entities.Add(new Seat
                {
                    Row = r,
                    Column = c,
                    AuditoriumId = auditoriumId
                });
            }
        }

        await _unitOfWork.seatRepository.AddRange(entities);

        await _unitOfWork.seatRepository.CommitAsync(ct);
    }

    public async Task ResetSeats(int auditoriumId, int rows, int columns, CancellationToken ct)
    {
        var existingSeats = _unitOfWork.seatRepository
            .Get(e => e.AuditoriumId == auditoriumId)
            .ToList();

        if (existingSeats.Any())
        {
            _unitOfWork.seatRepository.DeleteRange(existingSeats);

            await _unitOfWork.seatRepository.CommitAsync(ct);
        }

        await CreateSeats(auditoriumId, rows, columns, ct);
    }
}
