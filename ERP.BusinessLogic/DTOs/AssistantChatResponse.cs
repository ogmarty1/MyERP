namespace ERP.BusinessLogic.DTOs
{
    public class AssistantChatResponse
    {
        public bool Success { get; set; }
        public string Reply { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}
