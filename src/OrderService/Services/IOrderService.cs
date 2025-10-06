using Loft.Common.DTOs;
using Loft.Common.Enums;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderDTO> CreateOrder(OrderDTO order,IEnumerable<OrderItemDTO> items);
    Task<OrderDTO?> GetOrderById(long orderId);
    Task<IEnumerable<OrderDTO>> GetOrdersByCustomerId(long customerId, int page = 1, int pageSize = 20);
    Task UpdateOrderStatus(long orderId, ModerationStatus status);
    Task CancelOrder(long orderId);
    Task AddOrderItems(long orderId, OrderItemDTO items);
    Task RemoveOrderItems(long orderId, long orderItemId);
    Task<decimal> CalculateOrderTotal(IEnumerable<OrderItemDTO> items);
    
    
    /*
     * Примечания: CreateOrder возвращает созданный OrderDTO; CancelOrder переводит статус в
     * CANCELED и триггерит сопутствующие действия (refund/stock) на реализации.
     */
}