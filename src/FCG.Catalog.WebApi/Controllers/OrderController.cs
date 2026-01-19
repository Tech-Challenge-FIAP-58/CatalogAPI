using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Domain.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    public class OrderController(IOrderService service,
        ILogger<OrderController> logger) : StandardController
    {
		[Authorize(Roles = "Admin")]
        [HttpPost("RegisterOrder")]
        public Task<IActionResult> Create([FromBody] OrderRegisterDto dto)
        {
			logger.LogInformation("POST - Criar Pedido");
			return TryMethodAsync(() => service.Create(dto), logger);
		}
    }
}
