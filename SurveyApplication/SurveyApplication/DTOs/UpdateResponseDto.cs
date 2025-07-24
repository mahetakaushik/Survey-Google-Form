namespace SurveyApplication.DTOs
{
    public class UpdateResponseDto
    {

        public string Email { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new();
    }
}