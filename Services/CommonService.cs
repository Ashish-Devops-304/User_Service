using Contracts.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CommonService> logger;

        public CommonService(IHttpContextAccessor httpContextAccessor, ILogger<CommonService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        /// <summary>
        /// Allows access only if the user id from claims matches with the user id requested
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="ForbiddenException"></exception>
        public void UserIdAccessCheck(Guid userId)
        {
            logger.LogDebug("Received request to verify the access check for the user ID: " + userId);

            Guid userIdFromClaim = new Guid(httpContextAccessor.HttpContext.User?.FindFirstValue("user_id"));
            string? role = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (userIdFromClaim != userId && role == "Customer")
            {
                logger.LogError("User with ID: {0} has been denied to access the resource of the user with the ID: " +
                                  "{1} by returning Forbidden as response", userIdFromClaim, userId);

                throw new ForbiddenException("Access to this resource is denied"); 
            }

            logger.LogError("User with ID: {0} has been given access to their resource", userIdFromClaim);
        }

        /// <summary>
        /// Allows access only if the user id from claims matches with the user id from token
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void UserIdAccessCheckForAddressOrPayment(Guid userId)
        {
            try
            {
                UserIdAccessCheck(userId);
            }

            catch (ForbiddenException)
            {
                logger.LogError("User with ID: {0} has been denied to access the resource by returning Not Found response", userId);

                throw new NotFoundException("No user account has been found");
            }
        }

        /// <summary>
        /// Allows access only if the user is an admin
        /// </summary>
        /// <exception cref="ForbiddenException"></exception>
        public void AdminAccessCheck()
        {
            logger.LogDebug("Received request to verify if the user is an admin");

            string? role = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            Guid userIdFromClaim = new Guid(httpContextAccessor.HttpContext.User?.FindFirstValue("user_id"));


            if (role == "Customer")
            {
                logger.LogError("User with ID: {0} has been denied from accessing admin only resource", userIdFromClaim);

                throw new ForbiddenException("Access to this resource is denied");
            }

            logger.LogError("Admin with the ID: {0} has been given access to the resource", userIdFromClaim);

        }

        /// <summary>
        /// Gets user id from claims
        /// </summary>
        /// <returns></returns>
        public Guid GetUserIdFromClaims()
        {
            return new Guid(httpContextAccessor.HttpContext.User?.FindFirstValue("user_id"));
        }
    }
}
