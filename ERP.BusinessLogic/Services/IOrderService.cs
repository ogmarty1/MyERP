using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetFilteredAsync(OrderFilterRequest filter);
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderRequest request);
        Task ChangeStatusAsync(int orderId, OrderStatus status);
        Task CancelOrderAsync(int orderId);
        Task UpdateOrderItemsAsync(UpdateOrderItemsRequest request);
    }
}
