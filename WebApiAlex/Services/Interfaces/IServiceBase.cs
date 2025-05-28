using System.Collections.Generic;

namespace WebApiAlex.Services.Interfaces
{
    public interface IServiceBase<T>
    {
        List<T> GetAll();
        T GetById(Guid id);
        T Add(T entity);
        T Update(T entity);
        void Delete(Guid id);
    }
}
