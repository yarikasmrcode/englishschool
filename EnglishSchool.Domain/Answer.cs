namespace EnglishSchool.Domain
{
    public class Answer : BaseEntity
    {
        public string Content { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}