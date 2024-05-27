using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public class RepositoryBase<T> where T : class
    {
        private readonly HotelManagementContext _context;
        private readonly DbSet<T> _dbSet;
        public RepositoryBase(HotelManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }
        public void Add(T item)
        {
            _dbSet.Add(item);
            _context.SaveChanges();

        }
        public void Delete(T item)
        {
            _dbSet.Remove(item);
            _context.SaveChanges();
        }
        public void Update(T item)
        {
            _dbSet.Update(item);
            _context.SaveChanges();
        }
        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
