
namespace Ollio.Clients.Models.DuckDuckGo
{
    public class Root
    {
        public string Abstract { get; set; }
        public string AbstractSource { get; set; }
        public string AbstractText { get; set; }
        public string AbstractURL { get; set; }
        public string Answer { get; set; }
        public string AnswerType { get; set; }
        public string Definition { get; set; }
        public string DefinitionSource { get; set; }
        public string DefinitionURL { get; set; }
        public string Heading { get; set; }
        public string Image { get; set; }
        public string Redirect { get; set; }
        public Topic[] RelatedTopics { get; set; }
        public Topic[] Results { get; set; }
        public string Type { get; set; }
    }

    public class Icon
    {
        public string Height { get; set; }
        public string URL { get; set; }
        public string Width { get; set; }
    }

    public class Topic
    {
        public string FirstURL { get; set; }
        public Icon Icon { get; set; }
        public string Result { get; set; }
        public string Text { get; set; }
    }
}