namespace Resolute.ChatHub.Models
{
    public class SolutionTemplateViewModel
    {
        public string Intent { get; set; }
        public string Tasks { get; set; }
    }

    public class SolutionTemplate
    {
        public string Intent { get; set; }
        public object Tasks { get; set; }
        public object Actions { get; set; }
    }
}
