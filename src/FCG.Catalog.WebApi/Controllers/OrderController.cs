using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class OrderController(IOrderManagementService managementService,
        IOrderReadService readService,
        ILogger<OrderController> logger) : StandardController
    {
		[Authorize(Roles = "Admin")]
        [HttpPost("RegisterOrder")]
        public Task<IActionResult> Create([FromBody] OrderRegisterDto dto)
        {
           logger.LogInformation("POST - Create order");
           return TryMethodAsync(() => managementService.Create(dto), logger);
		}

        [Authorize]
        [HttpGet("GetOrderById/{id:guid}")]
        public Task<IActionResult> GetById(Guid id)
        {
            logger.LogInformation("GET - Get order by ID: {Id}", id);
            return TryMethodAsync(() => readService.GetById(id), logger);
        }

        [Authorize]
        [HttpGet("GetOrdersByUserId/{userId:int}")]
        public Task<IActionResult> GetByUserId(int userId)
        {
            logger.LogInformation("GET - Get orders by user ID: {UserId}", userId);
            return TryMethodAsync(() => readService.GetByUserId(userId), logger);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateOrder/{id:guid}")]
        public Task<IActionResult> Update(Guid id, [FromBody] OrderUpdateDto update)
        {
            logger.LogInformation("PUT - Update order with ID: {Id}", id);
            return TryMethodAsync(() => managementService.Update(id, update), logger);
        }
    }
}
