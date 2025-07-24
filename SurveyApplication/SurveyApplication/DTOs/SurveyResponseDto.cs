namespace SurveyApplication.DTOs
{
    public class SurveyResponseDto
    {
        public int SurveyId { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new();
    }
}
