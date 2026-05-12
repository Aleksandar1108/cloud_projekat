using Microsoft.AspNetCore.Mvc;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Text.Json;

namespace SmartGrid.WebApi.Extensions
{
    internal static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            return result.IsSuccess
                            ? new OkObjectResult(new { message = "Operation successful." })
                            : HandleFailure(result);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.IsSuccess
                            ? new OkObjectResult(result.Value)
                            : HandleFailure(result);
        }

        private static ObjectResult HandleFailure(Result result)
        {
            object finalMessage = result.Error?.Message ?? "An error occurred.";

            if (result.Error?.Type == ErrorType.Validation && !string.IsNullOrWhiteSpace(result.Error.Message))
            {
                try
                {
                    var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(result.Error.Message);
                    finalMessage = errors!;
                }
                catch
                {
                    finalMessage = result.Error.Message;
                }
            }

            string clientMessage;
            object? errorsPayload = null;
            if (result.Error?.Type == ErrorType.Validation)
            {
                if (finalMessage is Dictionary<string, string[]> dict)
                {
                    errorsPayload = dict;
                    clientMessage = "Validation failed.";
                }
                else
                    clientMessage = finalMessage.ToString() ?? result.Error.Message;
            }
            else
                clientMessage = finalMessage.ToString() ?? result.Error?.Message ?? "An error occurred.";

            var errorResponse = new
            {
                type = result.Error?.Type.ToString(),
                errors = errorsPayload,
                message = clientMessage
            };

            return result.Error?.Type switch
            {
                ErrorType.Validation => new BadRequestObjectResult(errorResponse),
                ErrorType.NotFound => new NotFoundObjectResult(errorResponse),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(errorResponse),
                ErrorType.Conflict => new ConflictObjectResult(errorResponse),
                ErrorType.Unexpected => new ObjectResult(errorResponse) { StatusCode = 500 },
                _ => new BadRequestObjectResult(errorResponse)
            };
        }
    }
}
