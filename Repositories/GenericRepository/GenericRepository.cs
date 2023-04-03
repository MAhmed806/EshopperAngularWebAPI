using EShopperAngular.Data;
using Microsoft.EntityFrameworkCore;

namespace EShopperAngular.Repositories.GenericRepository
{
    public class GenericRepository<T>:IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> Table = null;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            Table = _context.Set<T>();
        }
        public void Create(T obj)
        {
            Table.Add(obj);
        }

        public void Delete(int id)
        {
            T exists = Table.Find(id);
            Table.Remove(exists);
        }

        public T Details(int id)
        {
            return Table.Find(id);
        }

        public void Edit(T obj)
        {
            Table.Update(obj);
        }

        public IEnumerable<T> Index()
        {
            return Table.ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
