using EnglishSchool.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.DAL.Interfaces
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        Task<List<T>> GetAll(CancellationToken token);
        Task<T> GetById(int id, CancellationToken token);
        Task<bool> Create(T entity, CancellationToken token);
        Task<bool> Delete(int id);
        Task<bool> Update(T entity, CancellationToken token);
        Task<ICollection<T>> GetSubentities(int id, CancellationToken token);

    }
}
