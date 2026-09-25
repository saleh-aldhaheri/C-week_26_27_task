using week_26_27.ViewModels;

namespace week_26_27.Service.IService;

public interface IAuditoriumService
{
    public Task CreateAuditorium(int cinemaId, AuditoriumVM createAuditorium, CancellationToken ct);

    public Task<Dictionary<string, string>?> UpdateAuditorium(UpdateAuditoriumVM updateAuditoriumVm, CancellationToken ct);

    public Task<Dictionary<string, string>?> DeleteAuditorium(int auditoriumId, CancellationToken ct);

    public Auditorium? GetAuditorim(int id);
}
