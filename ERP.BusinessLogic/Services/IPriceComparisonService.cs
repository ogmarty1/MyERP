using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IPriceComparisonService
    {
        Task<PriceCheck> CheckPriceAsync(int productId);
        Task<List<PriceCheckOffer>> GetHistoryAsync(int productId);
    }
}
