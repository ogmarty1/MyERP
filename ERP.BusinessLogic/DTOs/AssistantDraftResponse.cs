namespace ERP.BusinessLogic.DTOs
{
    public class AssistantDraftResponse
    {
        public bool Success { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}
