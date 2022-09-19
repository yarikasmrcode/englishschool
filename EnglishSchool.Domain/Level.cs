namespace EnglishSchool.Domain
{
    public class Level : BaseEntity
    {
        public string Title { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public int MaterialId { get; set; }
        public Material Material { get; set; }
    }
}