using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace UserService.Controllers
{
    /// <summary>
    /// Handles all the customer delivery address related operations
    /// </summary>
    [Route("api/user/{userId:guid}/address")]
    [ApiController]
    [Authorize]
    public class UserAddressController : Controller
    {
        private readonly IUserAddressService userAddressService;
        private readonly ILogger<UserAddressController> logger;

        public UserAddressController(IUserAddressService userAddressService, ILogger<UserAddressController> logger)
        {
            this.userAddressService = userAddressService;
            this.logger = logger;
        }


        /// <summary>
        /// Stores delivery address of customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="address"></param>
        /// <response code="201">Address added successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpPost]
        [SwaggerResponse(statusCode: 201, description: "Address added successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult CreateAddress([FromRoute] Guid userId, [FromBody] AddressCreateDto address)
        {
            logger.LogInformation("Received request to add an address for the user ID: " + userId);

            return StatusCode(StatusCodes.Status201Created, userAddressService.CreateAddress(userId, address));
        }


        /// <summary>
        /// Gets all the customer delivery addresses by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<AddressResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllAddressByUserId([FromRoute] Guid userId)
        {
            logger.LogInformation("Received request to get all saved addresses for the user ID: " + userId);

            List<AddressResponseDto> userAddresses = userAddressService.GetAllAddressByUserId(userId);

            if (userAddresses.Count == 0)
            {
                logger.LogInformation("No saved address records found for the user ID: " + userId);

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Returning all saved address records for the user ID: {0}. Total records: {1}", userId, userAddresses.Count);

            return StatusCode(StatusCodes.Status200OK, userAddresses);
        }


        /// <summary>
        /// Gets an address by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No address has been found</response>
        [HttpGet("{addressId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(AddressResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No address has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetAddressByAddressId([FromRoute] Guid userId, [FromRoute] Guid addressId)
        {
            logger.LogInformation("Received request to get an address at {0} for the user ID: {1}", addressId, userId);

            return StatusCode(StatusCodes.Status200OK, userAddressService.GetAddressByAddressId(userId, addressId));
        }


        /// <summary>
        /// Updates an address by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <param name="updatedAddress"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No address has been found</response>
        [HttpPut("{addressId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No address has been found", type: typeof(ErrorResponseDto))]
        public IActionResult UpdateAddressByAddressId([FromRoute] Guid userId, [FromRoute] Guid addressId, AddressUpdateDto updatedAddress)
        {
            logger.LogInformation("Received request to update an address at {0} for the user ID: {1}", addressId, userId);

            userAddressService.UpdateAddressByAddressId(userId, addressId, updatedAddress);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes an address by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No address has been found</response>
        [HttpDelete("{addressId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No address has been found", type: typeof(ErrorResponseDto))]
        public IActionResult DeleteAddressByAddressId([FromRoute] Guid userId, [FromRoute] Guid addressId)
        {
            logger.LogInformation("Received request to delete an address at {0} for the user ID: {1}", addressId, userId);

            userAddressService.DeleteAddressByAddressId(userId, addressId);

            return StatusCode(StatusCodes.Status200OK);
        }

        // The below end points are specific only to inter-service communication

        /// <summary>
        /// Checks if the address id exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="404">No address has been found</response>
        [HttpGet("{addressId:guid}/verify")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 404, description: "No address has been found", type: typeof(ErrorResponseDto))]
        public IActionResult VerifyAddressId([FromRoute] Guid userId, [FromRoute] Guid addressId)
        {
            logger.LogInformation("Received request to verify an address at {0} for the user ID: {1}", addressId, userId);

            userAddressService.AddressIdExists(userId, addressId);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
