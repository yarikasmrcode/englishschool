namespace EnglishSchool.Domain
{
    public class Slide : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Question? Question { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}