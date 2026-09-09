using System.Linq.Expressions;

namespace week_26_27_task.Service;
public class ActorService : IActorService
{
    private IRepository<Actor> _repository;
    private IPagination _pagination;

    private IFileHelper _fileHelper;

    private readonly string _imageFilePath = "assets\\images\\actor";
    public ActorService(IRepository<Actor> repository, IPagination pagination, IFileHelper fileHelper)
    {
        _repository = repository;
        _pagination = pagination;
        _fileHelper = fileHelper;
    }

    public ActorWithFilterAndPaginationVM GetActors(ActorWithFilterAndPaginationVM actorsIndex)
    {
        var actors = _repository.Get();

        if (actorsIndex.Search is not null)
        {
            actors = actors.Where(
                e => e.FullName.ToLower().Contains(actorsIndex.Search.ToLower()) ||
                         e.Bio.ToLower().Contains(actorsIndex.Search.ToLower())
                );
        }

        int skip = (actorsIndex.Pagination.Page - 1) * actorsIndex.Pagination.PageSize;

        actorsIndex.Actors = actors.Skip(skip).Take(actorsIndex.Pagination.PageSize);

        actorsIndex.Pagination = _pagination
            .Paginate(
            actors.Count(),
            actorsIndex.Pagination.PageSize,
            actorsIndex.Pagination.Page
            );

        return actorsIndex;
    }

    public async Task CreateActor(Actor actor, IFormFile image, CancellationToken ct = default)
    {
     
        var name = _fileHelper.GenerateName(image.FileName);
        var path = _fileHelper.GeneratePath(name, _imageFilePath);

        if (path is null || !_fileHelper.Upload(path, image))
            throw new Exception();

        actor.Img = name;

        await _repository.Add(entity: actor);


        await _repository.CommitAsync(ct);
    }

    public async Task UpdateActor(Actor actor, CancellationToken ct = default, IFormFile? image = null)
    {
        Actor dbActor = GetActor(actor.Id);

        if (image is not null)
        {
            var name = _fileHelper.GenerateName(image.FileName);
            var path = _fileHelper.GeneratePath(name, _imageFilePath);

            if (path is null || !_fileHelper.Upload(path, image))
                throw new Exception();

            actor.Img = name;

            if (!string.IsNullOrEmpty(dbActor.Img))
            {
                var oldPath = _fileHelper.GeneratePath(dbActor.Img, _imageFilePath);
                if (oldPath is not null)
                    _fileHelper.Delete(oldPath);
            }
        }
        else
        {
            actor.Img = dbActor.Img;
        }

        _repository.Update(actor);

        await _repository.CommitAsync(ct);
    }

    public async Task DeleteActor(int id, CancellationToken ct = default)
    {
        Actor actor = GetActor(id);

        if (!string.IsNullOrEmpty(actor.Img))
        {
            var path = _fileHelper.GeneratePath(actor.Img, _imageFilePath);

            if (path is not null)
                _fileHelper.Delete(path);
        }

        _repository.Delete(actor);

        await _repository.CommitAsync(ct);
    }

    public Actor GetActor(int id)
    {
        var actor = _repository.GetOne(exprission: e => e.Id == id, false);

        if (actor is null)
            throw new Exception();

        return actor;
    }
    
    public IQueryable<Actor> GetAllActors(Expression<Func<Actor, bool>>? exprission = null)
    {
        return _repository.Get(exprission);
    }
}
