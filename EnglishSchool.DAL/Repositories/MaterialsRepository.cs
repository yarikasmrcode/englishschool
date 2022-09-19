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
    public class MaterialsRepository : RepositoryBase<Material>, IMaterialsRepository
    {
        public MaterialsRepository(AppDbContext context) : base(context)
        {
        }

      
    }
}
