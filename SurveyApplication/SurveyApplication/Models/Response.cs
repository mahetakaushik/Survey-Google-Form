namespace SurveyApplication.Models
{
    public class Response
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string email { get; set; }
        public bool IsDeleted { get; set; }
        public Survey Survey { get; set; }
        public List<ResponseAnswer> Answers { get; set; }
    }
}
