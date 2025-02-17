using Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace DataAccess
{
    public class Repository : IRepository
    {
        private readonly DatabaseContext _context;

        public Repository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ICollection<T>> GetAll<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<ICollection<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetSingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public async Task Add<T>(T entityToAdd) where T : class
        {
            await _context.Set<T>().AddAsync(entityToAdd);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
