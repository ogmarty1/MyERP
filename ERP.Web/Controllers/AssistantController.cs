using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    [Route("Assistant")]
    public class AssistantController : Controller
    {
        private readonly IAssistantService _assistantService;

        public AssistantController(IAssistantService assistantService)
        {
            _assistantService = assistantService;
        }

        [HttpPost("Chat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chat([FromBody] AssistantChatRequest request)
        {
            var result = await _assistantService.ChatAsync(request);
            return Json(result);
        }

        [HttpPost("Draft")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Draft([FromBody] AssistantDraftRequest request)
        {
            var result = await _assistantService.DraftAsync(request);
            return Json(result);
        }
    }
}
