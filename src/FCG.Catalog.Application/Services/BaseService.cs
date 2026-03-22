using FCG.Catalog.Domain.Web;
using FluentValidation.Results;
using System.Net;

namespace FCG.Catalog.Application.Services
{
    public abstract class BaseService
    {
        // ===== Prefer these =====
        protected ValidationResult ValidationResult;

        protected BaseService()
        {
            ValidationResult = new ValidationResult();
        }

        protected static IApiResponse<T> Ok<T>(T result, string message = "")
            => Build(result, HttpStatusCode.OK, true, message);

        protected static IApiResponse<T> Created<T>(T result, string message = "")
            => Build(result, HttpStatusCode.Created, true, message);

        // For operations without payload on success (Update/Delete)
        protected static IApiResponse<bool> NoContent(string message = "")
            => Build(false, HttpStatusCode.NoContent, true, message);

        // NotFound/BadRequest/etc. (kept - already correct)
        public static IApiResponse<T> BadRequest<T>(string message = "")
            => Build(default(T), HttpStatusCode.BadRequest, false, message);

        public static IApiResponse<T> NotFound<T>(string message = "")
            => Build(default(T), HttpStatusCode.NotFound, false, message);

        public static IApiResponse<T> Unauthorized<T>(string message = "")
            => Build(default(T), HttpStatusCode.Unauthorized, false, message);

        public static IApiResponse<T> InternalServerError<T>(string message = "")
            => Build(default(T), HttpStatusCode.InternalServerError, false, message);

        public static IApiResponse<T> RequestTimeout<T>(string message = "")
            => Build(default(T), HttpStatusCode.RequestTimeout, false, message);

        public static IApiResponse<T> GenericError<T>(HttpStatusCode statusCode, string message = "")
            => Build(default(T), statusCode, false, message);

        // ===== Backwards compatibility (avoid using going forward) =====
        // These keep the old behavior (status 200 always in "success")
        // Do not remove yet to avoid breaking existing code.
        public static IApiResponse<T> Success<T>(T resultValue, string message = "")
            => Build(resultValue, HttpStatusCode.OK, true, message);

        public static IApiResponse<bool> Success(string message = "")
            => Build(true, HttpStatusCode.OK, true, message);

        public static IApiResponse<T> Fail<T>(string message = "")
            => Build(default(T), HttpStatusCode.OK, false, message);
        // ==========================

        // Single factory
        private static IApiResponse<T> Build<T>(T? value, HttpStatusCode code, bool isSuccess, string message)
            => new ApiResponse<T>
            {
                ResultValue = value,
                Message = message,
                StatusCode = code,
                IsSuccess = isSuccess
            };

        // Simple concrete envelope implementation
        private sealed class ApiResponse<T> : IApiResponse<T>
        {
            public T? ResultValue { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;
            public bool IsSuccess { get; set; }
        }
    }
}
