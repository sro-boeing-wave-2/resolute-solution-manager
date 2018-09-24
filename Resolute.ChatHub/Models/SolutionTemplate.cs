using Newtonsoft.Json;

namespace Resolute.ChatHub.Models
{
    public class SolutionTemplateViewModel
    {
        public string Intent { get; set; }
        public string Tasks { get; set; }
    }

    public class SolutionTemplate
    {
        [JsonIgnore]
        public string _id { get; set; }
        public string Intent { get; set; }
        public object Tasks { get; set; }
        public string Actions { get; set; }
    }
}
