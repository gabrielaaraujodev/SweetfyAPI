using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.OrderDTO;
using SweetfyAPI.Services;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages orders (or quotes) for the logged-in user's bakery.
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/>.
        /// </summary>
        /// <param name="orderService">The service responsible for order logic.</param>
        /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a simplified list of all orders for the user's bakery.
        /// </summary>
        /// <returns>A list of simple OrderDto objects.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var orders = await _orderService.GetOrdersForUserAsync();
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        /// <summary>
        /// Gets the full details of a specific order, including its products and recipes.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The complete OrderDetailsDto.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int id)
        {
            var order = await _orderService.GetOrderDetailsByIdForUserAsync(id);
            if (order == null)
                return NotFound(new { Message = "Order not found." });

            return Ok(_mapper.Map<OrderDetailsDto>(order));
        }

        /// <summary>
        /// Creates a new order. This operation is transactional and 
        /// links all specified products/recipes.
        /// </summary>
        /// <param name="dto">The composite DTO containing order data and its items.</param>
        /// <returns>The newly created order with all its details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDetailsDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var newOrder = await _orderService.CreateOrderAsync(dto);
            if (newOrder == null)
                return BadRequest(new { Message = "Failed to create order." });

            var readDto = _mapper.Map<OrderDetailsDto>(newOrder);
            return CreatedAtAction(nameof(GetOrderDetails), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing order's main details (e.g., Name, Description, Status). 
        /// This does NOT update the order's items.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="dto">The DTO containing the order's top-level data to update.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, dto);
            if (updatedOrder == null)
                return NotFound(new { Message = "Order not found." });

            return NoContent();
        }

        /// <summary>
        /// Deletes an order and all its associated items (order products/recipes) by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content (204) on success.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound(new { Message = "Order not found." });

            return NoContent();
        }
    }
}