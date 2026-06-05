using System.ComponentModel.DataAnnotations;

namespace MyAOS.Domain.Dto
{
    public class SendWhitelistEmailRequest
    {
        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required.")]
        public string Body { get; set; } = string.Empty;
    }

    public class SendWhitelistEmailResponse
    {
        public int SentCount { get; set; }
        public List<string> SentTo { get; set; } = new();
        public List<string> FailedTo { get; set; } = new();
        public Dictionary<string, string> FailedDetails { get; set; } = new();
    }
}
