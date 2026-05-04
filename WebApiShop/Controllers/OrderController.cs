using DTOs;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Service;
using WebApiShop.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase, IOrderController
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [AuthorizeAdmin]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            if (orders == null || !orders.Any())
                return NotFound(new { Message = $"No orders found for user {userId}" });

            return Ok(orders);
        }

        // POST api/<OrderController>
        [Authorize]
        [HttpPost]
        async public Task<ActionResult<OrderReadDTO>> Post([FromBody] OrderCreateDTO order)
        {
            var newOrder = await _orderService.addOrder(order);
            if (newOrder == null)
                return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = newOrder.OrderId }, newOrder);
        }

        // PUT api/<OrderController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ChangeOrderStatusDto dto)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(id, dto);
                if (!updated)
                    return NotFound(new { Message = $"Order with id {id} not found." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE api/<OrderController>/5
        [AuthorizeAdmin]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
