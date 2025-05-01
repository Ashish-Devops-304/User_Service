using Entities.Dtos;
using Services;
using System.Text.Json;

namespace User_Service.Helpers
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }

            catch (BadRequestException b)
            {
                await context.Response.WriteAsync(HandleException(400, "Bad Request", b.Message, context));
            }

            catch (UnauthorizedException u)
            {
                await context.Response.WriteAsync(HandleException(401, "Unauthorized", u.Message, context));
            }

            catch (ForbiddenException f)
            {
                await context.Response.WriteAsync(HandleException(403, "Forbidden", f.Message, context));
            }

            catch (NotFoundException n)
            {
                await context.Response.WriteAsync(HandleException(404, "Not found", n.Message, context));
            }

            catch (ConflictException c)
            {
                await context.Response.WriteAsync(HandleException(409, "Conflict", c.Message, context));
            }

            catch (Exception ex)
            {
                await context.Response.WriteAsync(HandleException(500, "An error occurred while processing your request", ex.Message, context));
            }
        }

        /// <summary>
        /// Handles all the catch block operations 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="description"></param>
        /// <param name="context"></param>
        /// <returns></returns>

        public string HandleException(int statusCode, string message, string description, HttpContext context)
        {
            // Uses ErrorResponseDto to set status code, message and description for the condition

            ErrorResponseDto errorResponse = new ErrorResponseDto() { StatusCode = statusCode, Message = message, Description = description };

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };

            string json = JsonSerializer.Serialize(errorResponse, options);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return json;
        }
    }
}
