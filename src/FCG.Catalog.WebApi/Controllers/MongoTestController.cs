using FCG.Catalog.Domain.Common;
using FCG.Catalog.Infra.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
	public class MongoTestController(IEventLogRepository eventLogRepository) : StandardController
	{
		[HttpPost]
		public async Task<IActionResult> InsertCartEventLog(CartEventLog eventLog)
		{
			await eventLogRepository.InsertCartEventLog(eventLog);
			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> GetByUserId()
		{
			var logs = await eventLogRepository.GetCartEnvetLogs();
			return Ok(logs);
		}
	}
}
