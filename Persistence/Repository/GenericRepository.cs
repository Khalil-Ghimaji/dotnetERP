namespace Persistence.Repository;

public interface GenericRepository<T> where T:class
{
    public Task<T> GetById(Guid id);
    public Task<IEnumerable<T>> GetAll();
    public Task<T> Add(T entity);
    public Task<T> Update(T entity);
    public Task<T> Delete(Guid id);
}