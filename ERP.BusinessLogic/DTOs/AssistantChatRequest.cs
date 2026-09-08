namespace ERP.BusinessLogic.DTOs
{
    public class AssistantChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<ChatMessageDto> History { get; set; } = new();
    }
}
