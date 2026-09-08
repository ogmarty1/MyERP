using ERP.BusinessLogic.DTOs;

namespace ERP.BusinessLogic.Services
{
    public interface IAssistantService
    {
        Task<AssistantChatResponse> ChatAsync(AssistantChatRequest request);
        Task<AssistantDraftResponse> DraftAsync(AssistantDraftRequest request);
    }
}
