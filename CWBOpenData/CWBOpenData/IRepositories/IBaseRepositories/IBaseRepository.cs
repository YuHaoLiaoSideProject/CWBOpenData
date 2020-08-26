using System.Collections.Generic;

namespace CWBOpenData.IRepositories.IBaseRepositories
{
    public interface IBaseRepository<T>
    {
        bool Create(T model);
        List<T> GetList();

        T Get<S>(S id);

        bool Create(List<T> models);

        void Update(List<T> models);

        bool Update(T model);

        void CreateOrUpdate(List<T> models);

        void CreateOrUpdate(T model);
    }
}
