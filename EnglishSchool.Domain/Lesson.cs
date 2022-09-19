using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.Domain
{
    public class Lesson : BaseEntity
    {
        public string Name { get; set; }
        public Level Level { get; set; }
        public ICollection<Slide> Slides { get; set; }
    }
}
