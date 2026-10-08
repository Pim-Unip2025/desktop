using System;
using TronusDesktop.Models;

namespace TronusDesktop.Services
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public ApiErrorResponse ErrorResponse { get; }

        public ApiException(int statusCode, ApiErrorResponse errorResponse)
            : base(errorResponse?.GetErrorMessage() ?? $"Erro de API (Status: {statusCode})")
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse;
        }
    }
}
