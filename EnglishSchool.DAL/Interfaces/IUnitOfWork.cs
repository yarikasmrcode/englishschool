using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IMaterialsRepository Materials { get; }
        Task CompleteAsync();
    }
}
