using System.Security.Claims;
using backend.Dto;
using backend.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IShopDao _dao;
        public OrdersController(IShopDao dao) => _dao = dao;

        long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Buy([FromBody] BuyProductRequestDto dto)
            => Ok(await _dao.BuyAsync(UserId, dto));

        [HttpGet]
        public async Task<IActionResult> MyOrders()
            => Ok(await _dao.GetUserOrdersAsync(UserId));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Detail(long id)
        {
            var data = await _dao.GetOrderDetailAsync(UserId, id);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> UpdateOrderStatus(long id, [FromBody] OrderStatusUpdateDto dto)
            => await _dao.UpdateOrderStatusAsync(UserId, id, dto.Status) ? NoContent() : NotFound();

        [HttpPatch("{id:long}/shipment/status")]
        public async Task<IActionResult> UpdateShipmentStatus(long id, [FromBody] ShipmentStatusUpdateDto dto)
            => await _dao.UpdateShipmentStatusAsync(UserId, id, dto.Status) ? NoContent() : NotFound();
    }
}
