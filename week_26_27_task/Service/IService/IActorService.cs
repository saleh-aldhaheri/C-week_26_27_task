using System.Linq.Expressions;

namespace week_26_27_task.Service.IService;

public interface IActorService
{
    ActorWithFilterAndPaginationVM GetActors(ActorWithFilterAndPaginationVM ActorsIndex);

    Task CreateActor(Actor Actor, IFormFile image, CancellationToken ct = default);

    Task UpdateActor(Actor Actor, CancellationToken ct = default, IFormFile? image = null);

    Task DeleteActor(int id, CancellationToken ct = default);

    Actor GetActor(int id);

    public IQueryable<Actor> GetAllActors(Expression<Func<Actor, bool>>? exprission = null);
}
