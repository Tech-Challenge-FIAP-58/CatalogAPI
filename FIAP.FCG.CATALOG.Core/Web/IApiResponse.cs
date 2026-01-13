using System.Net;

namespace FIAP.FCG.CATALOG.Core.Web
{
	public interface IApiResponse<TResult>
	{
		TResult? ResultValue { get; set; }
		HttpStatusCode StatusCode { get; set; }
		string Message { get; set; }
		bool IsSuccess { get; set; }
	}
}
