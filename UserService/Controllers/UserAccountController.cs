using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using User_Service.Helpers;

namespace UserService.Controllers
{
    /// <summary>
    /// Handles all the user account related operations
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserAccountController : Controller
    {
        private readonly IUserAccountService userAccountService;
        private readonly ILogger<UserAccountController> logger;

        public UserAccountController (IUserAccountService userAccountService, ILogger<UserAccountController> logger)
        {
            this.userAccountService = userAccountService;
            this.logger = logger;
        }

        /// <summary>
        /// Creates a user account as a customer/admin
        /// </summary>
        /// <param name="newUserAccount"></param>
        /// <response code="201">User account created successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="409">User account already exists</response>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 201, description: "User account created successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "User account already exists", type: typeof(ErrorResponseDto))]
        public IActionResult CreateUserAccount([FromBody] UserCreateDto newUserAccount)
        {
            logger.LogInformation("Received request to create user account");

            IdentityResponseDto createdAccount = userAccountService.CreateUserAccount(newUserAccount);

            logger.LogInformation("New user account created with the Id: " + createdAccount.Id);

            return StatusCode(StatusCodes.Status201Created, createdAccount);
        }


        /// <summary>
        /// Authenticates the user and returns token upon successfull authentication
        /// </summary>
        /// <param name="userLogIn"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(TokenResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        public IActionResult Login([FromBody] UserLogInDto userLogIn)
        {
            logger.LogInformation("User with the email address: {0} is requesting for login access", userLogIn.Email);

            TokenResponseDto accessToken = userAccountService.AuthenticateUser(userLogIn);

            logger.LogInformation("User with the email address: {0} is successfully authenticated and a token is returned", userLogIn.Email);

            return StatusCode(StatusCodes.Status200OK, accessToken);
        }


        /// <summary>
        /// Get the complete user account details by user id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(UserAccountResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 403, description: "Access to this resource is denied", type: typeof(ErrorResponseDto))]
        public IActionResult GetUserAccountById([FromRoute] Guid id)
        {
            logger.LogInformation("Received request to get user account details for the ID: " + id);

            UserAccountResponseDto userAccount = userAccountService.GetUserAccountById(id);

            logger.LogInformation("Returned complete user account information for the user ID: " + id);

            return StatusCode(StatusCodes.Status200OK, userAccount);
        }


        /// <summary>
        /// Admins can get all the user account details 
        /// </summary>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="403">Customers don't have access to this resource</response>
        [HttpGet]
        [AdminAccess]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<UserAccountOnlyResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 403, description: "Access to this resource is denied", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllUserAccount()
        {
            logger.LogInformation("Received request to get all user account details");

            List<UserAccountOnlyResponseDto> userAccounts = userAccountService.GetAllUserAccount();

            if (userAccounts.Count == 0)
            {
                logger.LogInformation("No user account records found. Returning 0 records.");

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Returning all user account records. Total records: {0}", userAccounts.Count);

            return StatusCode(StatusCodes.Status200OK, userAccounts);
        }


        /// <summary>
        /// Update user account details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userAccount"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response> 
        /// <response code="404">No user account has been found</response>
        /// <response code="409">User account already exists</response>
        [HttpPut("{id:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "User account already exists", type: typeof(ErrorResponseDto))]
        public IActionResult UpdateUserAccountById ([FromRoute] Guid id, [FromBody] UserUpdateDto userAccount)
        {
            logger.LogInformation("Received request to update an user account for the ID: " + id);

            userAccountService.UpdateUserAccount(id, userAccount);

            logger.LogInformation("User account information has been successfully updated for the Id: " + id);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a user account by user id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response> 
        /// <response code="404">No user account has been found</response>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult DeleteUserAccountById([FromRoute] Guid id)
        {
            logger.LogInformation("Received request to delete an user account for the ID: " + id);

            userAccountService.DeleteUserAccount(id);

            logger.LogInformation("User account deleted for the Id: " + id);

            return StatusCode(StatusCodes.Status200OK);
        }

        // The below end points are specific only to inter-service communication

        /// <summary>
        /// Checks if a user id exists in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet("{userId}/verify")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult VerifyUserId(Guid userId)
        {
            logger.LogInformation("Received request to verify an user account for the ID: " + userId);

            userAccountService.UserIdExists(userId);

            logger.LogInformation("User Id: {0} has been successfully verified", userId);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
} 