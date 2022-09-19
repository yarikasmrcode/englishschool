using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishSchool.Domain
{
    public class Question : BaseEntity
    {
        public string Title { get; set; }
        public int SlideId { get; set; }
        public Slide Slide { get; set; }
        public Answer Answer { get; set; }
    }
}
