namespace week_26_27.Service.IService;

public interface ISeatService
{
    public Task CreateSeats(int auditoriumId, int rows, int columns, CancellationToken ct);

    public Task ResetSeats(int auditoriumId, int rows, int columns, CancellationToken ct);
}
