namespace EShopperAngular.Repositories.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Index();
        T Details(int id);
        void Create(T obj);
        void Edit(T obj);
        void Delete(int id);
        void Save();
    }
}
