using System.Text.Json.Serialization;

namespace SurveyApplication.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }

        public int SurveyId { get; set; }

        [JsonIgnore]
        public Survey? Survey { get; set; }

        public List<Option> Options { get; set; }

    }
}
