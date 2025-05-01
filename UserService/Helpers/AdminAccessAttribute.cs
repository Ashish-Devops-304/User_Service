using Microsoft.AspNetCore.Mvc.Filters;
using Services;

namespace User_Service.Helpers
{
    /// <summary>
    /// Gives access only if the user is an admin
    /// </summary>
    public class AdminAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? role = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type ==
                           "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            if (role == "Customer")
            {
                throw new ForbiddenException("Sorry, only admins has access to this resource :(");
            }
        }
    }
}
