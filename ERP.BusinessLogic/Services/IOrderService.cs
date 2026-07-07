using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest request);
    }
}
