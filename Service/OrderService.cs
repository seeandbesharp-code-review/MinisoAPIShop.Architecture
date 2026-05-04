using AutoMapper;
using DTOs;
using Entities;
using Order = Entities.Order;
using Repository;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IDatabase _cache;
        private readonly IConfiguration _configuration;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _cache = redis.GetDatabase();
            _configuration = configuration;
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrdersAsync()
        {
            string cacheKey = "orders_all";

            var cachedOrders = await _cache.StringGetAsync(cacheKey);
            if (!cachedOrders.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<OrderReadDTO>>(cachedOrders);
            }

            var orders = await _orderRepository.GetAllOrdersAsync();
            var ordersDto = _mapper.Map<IEnumerable<OrderReadDTO>>(orders);
            var ttlMinutes = _configuration.GetValue<int>("CacheSettings:DefaultTTLInMinutes");

            string jsonOrders = JsonSerializer.Serialize(ordersDto);
            await _cache.StringSetAsync(cacheKey, jsonOrders, TimeSpan.FromMinutes(ttlMinutes));

            return ordersDto;
        }

        public async Task<IEnumerable<OrderReadDTO>> GetOrdersByUserIdAsync(int userId)
        {
            string cacheKey = $"orders_userId_{userId}";

            var cachedOrders = await _cache.StringGetAsync(cacheKey);
            if (!cachedOrders.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<OrderReadDTO>>(cachedOrders);
            }

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var ordersDto = _mapper.Map<List<OrderReadDTO>>(orders);

            var ttlMinutes = _configuration.GetValue<int>("CacheSettings:DefaultTTLInMinutes");
            string jsonOrders = JsonSerializer.Serialize(ordersDto);
            await _cache.StringSetAsync(cacheKey, jsonOrders, TimeSpan.FromMinutes(ttlMinutes));

            return ordersDto;
        }

        public async Task<OrderReadDTO> addOrder(OrderCreateDTO orderDto)
        {
            Order order = _mapper.Map<Order>(orderDto);
            order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            order.Status = "Accepted";
            Order newOrder = await _orderRepository.AddOrder(order);

            await _cache.KeyDeleteAsync("orders_all");
            await _cache.KeyDeleteAsync($"orders_userId_{newOrder.UserId}");

            return _mapper.Map<OrderReadDTO>(newOrder);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, ChangeOrderStatusDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            if (order.Status == "Received")
                throw new InvalidOperationException("אי אפשר לשנות אחרי Received");

            if (dto.Received)
            {
                if (order.Status != "Delivered")
                    throw new InvalidOperationException("אפשר לסמן קיבלתי רק אחרי Delivered");

                var result = await _orderRepository.UpdateOrderStatusAsync(orderId, "Received");
                if (result)
                {
                    await _cache.KeyDeleteAsync("orders_all");
                    await _cache.KeyDeleteAsync($"orders_userId_{order.UserId}");
                }
                return result;
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status חסר");

            var allowed = new[] { "Accepted", "Processing", "Shipped", "Delivered" };

            if (!allowed.Contains(dto.Status))
                throw new ArgumentException("סטטוס לא חוקי");

            var updateResult = await _orderRepository.UpdateOrderStatusAsync(orderId, dto.Status);
            if (updateResult)
            {
                await _cache.KeyDeleteAsync("orders_all");
                await _cache.KeyDeleteAsync($"orders_userId_{order.UserId}");
            }
            return updateResult;
        }
    }
}