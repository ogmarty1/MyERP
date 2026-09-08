namespace ERP.BusinessLogic.DTOs
{
    public class AssistantDraftRequest
    {
        public string Instruction { get; set; } = string.Empty;
        public string? Context { get; set; }
    }
}
