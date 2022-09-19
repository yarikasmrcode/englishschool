namespace EnglishSchool.Domain
{
    public class Material : BaseEntity
    {
        public string Title { get; set; }
        public ICollection<Level> Levels { get; set; }
    }
}