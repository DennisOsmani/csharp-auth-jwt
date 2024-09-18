﻿using exercise.wwwapi.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace exercise.wwwapi.Repositories
{
    public class DatabaseRepository<T> : IDatabaseRepository<T> where T : class
    {
        private BlogContext _db;
        private DbSet<T> _table = null;
        public DatabaseRepository(BlogContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includeExpressions)
        {
            if (includeExpressions.Any())
            {
                var set = includeExpressions
                    .Aggregate<Expression<Func<T, object>>, IQueryable<T>>
                     (_table, (current, expression) => current.Include(expression));
            }
            return await _table.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _table.ToListAsync();
        }
        public async Task<T> GetById(object id)
        {
            return await _table.FindAsync(id);
        }

        public async Task Insert(T obj)
        {
            await _table.AddAsync(obj);
        }
        public async Task Update(T obj)
        {
            _table.Attach(obj);
            _db.Entry(obj).State = EntityState.Modified;
        }

        public async Task Delete(object id)
        {
            T existing = await _table.FindAsync(id);
            _table.Remove(existing);
        }

        public async Task Save()
        {
           await _db.SaveChangesAsync();
        }
        public DbSet<T> Table { get { return _table; } }
    }
}
