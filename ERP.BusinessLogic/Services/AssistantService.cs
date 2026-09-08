using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;
using Microsoft.Extensions.Configuration;

namespace ERP.BusinessLogic.Services
{
    public class AssistantService : IAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly IDashboardService _dashboardService;
        private readonly string _baseUrl;
        private readonly string _model;

        public AssistantService(HttpClient httpClient, IDashboardService dashboardService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _dashboardService = dashboardService;
            _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";
            _model = configuration["Ollama:Model"] ?? "llama3.1:8b";
        }

        public async Task<AssistantChatResponse> ChatAsync(AssistantChatRequest request)
        {
            try
            {
                var systemPrompt = await BuildSystemPromptAsync();

                var messages = new List<OllamaChatMessage> { new("system", systemPrompt) };
                messages.AddRange(request.History.Select(h => new OllamaChatMessage(h.Role, h.Content)));
                messages.Add(new OllamaChatMessage("user", request.Message));

                var reply = await SendToOllamaAsync(messages);

                return new AssistantChatResponse { Success = true, Reply = reply };
            }
            catch (Exception ex)
            {
                return new AssistantChatResponse
                {
                    Success = false,
                    Error = "The assistant is not available right now. Make sure Ollama is running locally. (" + ex.Message + ")"
                };
            }
        }

        public async Task<AssistantDraftResponse> DraftAsync(AssistantDraftRequest request)
        {
            try
            {
                const string systemPrompt = "You are a concise writing assistant embedded in an ERP system. " +
                    "Write only the requested text itself, with no preamble, quotation marks, or explanation. " +
                    "Keep it professional and no more than 2-3 sentences unless asked otherwise.";

                var userPrompt = request.Instruction;
                if (!string.IsNullOrWhiteSpace(request.Context))
                    userPrompt += "\n\nContext:\n" + request.Context;

                var messages = new List<OllamaChatMessage>
                {
                    new("system", systemPrompt),
                    new("user", userPrompt)
                };

                var text = await SendToOllamaAsync(messages);

                return new AssistantDraftResponse { Success = true, Text = text.Trim().Trim('"') };
            }
            catch (Exception ex)
            {
                return new AssistantDraftResponse
                {
                    Success = false,
                    Error = "The assistant is not available right now. Make sure Ollama is running locally. (" + ex.Message + ")"
                };
            }
        }

        private async Task<string> SendToOllamaAsync(List<OllamaChatMessage> messages)
        {
            var body = new OllamaChatRequestBody(_model, messages, false);
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/chat", body);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaChatResponseBody>();
            return result?.Message?.Content?.Trim() ?? string.Empty;
        }

        private async Task<string> BuildSystemPromptAsync()
        {
            var dashboard = await _dashboardService.GetManagerDashboardAsync();

            var lowStockLines = dashboard.LowStockProducts.Select(p =>
            {
                var suggestedReorderQty = (p.MinimumQuantity * 2) - p.QuantityInStock;
                return $"- {p.Name} (SKU {p.SKU}): {p.QuantityInStock} in stock, minimum {p.MinimumQuantity}, suggested reorder quantity {suggestedReorderQty}";
            });
            var lowStockBlock = dashboard.LowStockProducts.Count == 0
                ? "None."
                : string.Join("\n", lowStockLines);

            var topProductsBlock = dashboard.TopSellingProducts.Count == 0
                ? "None."
                : string.Join("\n", dashboard.TopSellingProducts.Select(p => $"- {p.ProductName}: {p.Revenue:0.00} EUR revenue"));

            return "You are the built-in AI assistant for MyERP, an ERP system used by a small business. " +
                   "Answer questions using ONLY the business data snapshot below plus general business reasoning. " +
                   "All monetary values are in Euro (EUR). Be concise and use plain language. " +
                   "If asked something the data below cannot answer, say so honestly instead of guessing exact figures.\n\n" +
                   "=== Current business data snapshot ===\n" +
                   $"Current month revenue: {dashboard.MonthlyRevenue:0.00} EUR\n" +
                   $"New orders this month: {dashboard.NewMonthlyOrdersCount}\n" +
                   $"Active customers: {dashboard.TotalActiveCustomers}\n" +
                   $"Products currently low on stock: {dashboard.CriticalStockCount}\n\n" +
                   "Low stock products (name, current qty, minimum qty, suggested reorder qty):\n" +
                   lowStockBlock + "\n\n" +
                   "Top selling products this period:\n" +
                   topProductsBlock;
        }

        private record OllamaChatMessage(
            [property: JsonPropertyName("role")] string Role,
            [property: JsonPropertyName("content")] string Content);

        private record OllamaChatRequestBody(
            [property: JsonPropertyName("model")] string Model,
            [property: JsonPropertyName("messages")] List<OllamaChatMessage> Messages,
            [property: JsonPropertyName("stream")] bool Stream);

        private class OllamaChatResponseBody
        {
            [JsonPropertyName("message")]
            public OllamaChatMessage? Message { get; set; }
        }
    }
}
