using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ERP.BusinessLogic.Services
{
    public class PriceComparisonService : IPriceComparisonService
    {
        private const int MaxOffers = 10;

        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly string? _apiKey;
        private readonly string _googleDomain;
        private readonly string _country;

        public PriceComparisonService(HttpClient httpClient, ApplicationDbContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _apiKey = configuration["SerpApi:ApiKey"];
            // "bg" (Bulgaria) is not a supported Google Shopping `gl` country on SerpApi - it 400s.
            // Germany gives real EUR-priced EU retailer results, matching this app's EUR-only convention.
            _googleDomain = configuration["SerpApi:GoogleDomain"] ?? "google.de";
            _country = configuration["SerpApi:Country"] ?? "de";
        }

        public async Task<PriceCheck> CheckPriceAsync(int productId)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("SerpApi API key is not configured.");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            var query = product.Name;
            var url = "https://serpapi.com/search.json" +
                      $"?engine=google_shopping" +
                      $"&q={Uri.EscapeDataString(query)}" +
                      $"&google_domain={Uri.EscapeDataString(_googleDomain)}" +
                      $"&gl={Uri.EscapeDataString(_country)}" +
                      $"&api_key={Uri.EscapeDataString(_apiKey)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SerpApiShoppingResponse>();

            var priceCheck = new PriceCheck
            {
                ProductId = productId,
                CheckedAt = DateTime.UtcNow,
                Query = query
            };

            var offers = (result?.ShoppingResults ?? new List<SerpApiShoppingResult>())
                .Where(r => r.ExtractedPrice.HasValue && !string.IsNullOrWhiteSpace(r.Source))
                .Take(MaxOffers)
                .Select(r => new PriceCheckOffer
                {
                    SourceName = r.Source!,
                    Price = r.ExtractedPrice!.Value,
                    Link = r.Link,
                    Position = r.Position
                });

            foreach (var offer in offers)
                priceCheck.Offers.Add(offer);

            _context.PriceChecks.Add(priceCheck);
            await _context.SaveChangesAsync();

            return priceCheck;
        }

        public async Task<List<PriceCheckOffer>> GetHistoryAsync(int productId)
        {
            return await _context.PriceCheckOffers
                .Include(o => o.PriceCheck)
                .Where(o => o.PriceCheck.ProductId == productId)
                .OrderByDescending(o => o.PriceCheck.CheckedAt)
                .ThenBy(o => o.Position)
                .ToListAsync();
        }

        private class SerpApiShoppingResponse
        {
            [JsonPropertyName("shopping_results")]
            public List<SerpApiShoppingResult>? ShoppingResults { get; set; }
        }

        private class SerpApiShoppingResult
        {
            [JsonPropertyName("position")]
            public int Position { get; set; }

            [JsonPropertyName("source")]
            public string? Source { get; set; }

            [JsonPropertyName("product_link")]
            public string? Link { get; set; }

            [JsonPropertyName("extracted_price")]
            public decimal? ExtractedPrice { get; set; }
        }
    }
}
