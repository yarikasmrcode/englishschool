using EnglishSchool.DAL.Interfaces;
using EnglishSchool.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.DAL.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Create(T entity, CancellationToken token)
        {
            await _context.Set<T>().AddAsync(entity, token);

            return await Save();
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            _context.Remove(entity);

            return await Save();
        }

        public async Task<List<T>> GetAll(CancellationToken token)
        {
            return await _context.Set<T>().ToListAsync(token);
        }

        public async Task<T> GetById(int id, CancellationToken token)
        {
            var entity = await _context.Set<T>().FindAsync(id, token);

            return entity;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();

            if (saved > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<ICollection<T>> GetSubentities(int id, CancellationToken token)
        {
            return await _context.Set<T>().Where(x => x.Id == id).ToListAsync(token);
        }

        public async Task<bool> Update(T entity, CancellationToken token)
        {
            _context.Set<T>().Update(entity);

            return await Save();
        }
    }
}
