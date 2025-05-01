using Contracts.IServices;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace UserService.Controllers
{
    /// <summary>
    /// Handles all the customer payment related operations
    /// </summary>
    [Route("api/user/{userId:guid}/payment")]
    [ApiController]
    [Authorize]
    public class UserPaymentController : Controller
    {
        private readonly IUserPaymentService userPaymentService;
        private readonly ILogger<UserPaymentController> logger;

        public UserPaymentController(IUserPaymentService userPaymentService, ILogger<UserPaymentController> logger)
        {
            this.userPaymentService = userPaymentService;
            this.logger = logger;
        }

        /// <summary>
        /// Stores payment details of a customer
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPayment"></param>
        /// <response code="201">Payment detail added successfully</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="409">Payment name/value detail already exists</response>
        [HttpPost]
        [SwaggerResponse(statusCode: 201, description: "Payment detail added successfully", type: typeof(IdentityResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Payment name/value detail already exists", type: typeof(ErrorResponseDto))]
        public IActionResult CreatePaymentDetail([FromRoute] Guid userId, [FromBody] PaymentCreateDto newPayment)
        {
            logger.LogInformation("Received request to add new payment detail for the user ID: " + userId);

            return StatusCode(StatusCodes.Status201Created, userPaymentService.CreatePaymentDetail(userId, newPayment));
        }


        /// <summary>
        /// Gets all payment detail of the user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="204">No content has been found</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        [HttpGet]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(List<PaymentResponseDto>))]
        [SwaggerResponse(statusCode: 204, description: "No content has been found")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetAllPaymentByUserId([FromRoute] Guid userId)
        {
            logger.LogInformation("Received request to get all saved payment details for the user ID: " + userId);

            List<PaymentResponseDto> userPayment = userPaymentService.GetAllPaymentByUserId(userId);

            if (userPayment.Count == 0)
            {
                logger.LogInformation("No saved payment records found for the user ID: " + userId);

                return StatusCode(StatusCodes.Status204NoContent);
            }

            logger.LogInformation("Returning all saved payment records for the user ID: {0}. Total records: {1}", userId, userPayment.Count);

            return StatusCode(StatusCodes.Status200OK, userPayment);
        }


        /// <summary>
        /// Gets an payment detail by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No payment detail has been found</response>
        [HttpGet("{paymentId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation", type: typeof(PaymentResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No payment detail has been found", type: typeof(ErrorResponseDto))]
        public IActionResult GetPaymentByPaymentId([FromRoute] Guid userId, [FromRoute] Guid paymentId)
        {
            logger.LogInformation("Received request to get a payment detail at {0} for the user ID: {1}", paymentId, userId);

            return StatusCode(StatusCodes.Status200OK, userPaymentService.GetPaymentByPaymentId(userId, paymentId));
        }


        /// <summary>
        /// Updates an payment detail by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <param name="updatedPayment"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="400">Given input is invalid</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No payment detail has been found</response>
        /// <response code="409">Payment name/value detail already exists</response>
        [HttpPut("{paymentId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 400, description: "Given input is invalid", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No payment detail has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 409, description: "Payment name/value detail already exists", type: typeof(ErrorResponseDto))]
        public IActionResult UpdatePaymentByPaymentId([FromRoute] Guid userId, [FromRoute] Guid paymentId, PaymentUpdateDto updatedPayment)
        {
            logger.LogInformation("Received request to update a payment detail at {0} for the user ID: {1}", paymentId, userId);

            userPaymentService.UpdatePaymentByPaymentId(userId, paymentId, updatedPayment);

            return StatusCode(StatusCodes.Status200OK);
        }


        /// <summary>
        /// Deletes a payment detail by its id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="401">Lacks valid authentication credentials</response>
        /// <response code="404">No user account has been found</response>
        /// <response code="404">No address has been found</response>
        [HttpDelete("{paymentId:guid}")]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 401, description: "Lacks valid authentication credentials", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No user account has been found", type: typeof(ErrorResponseDto))]
        [SwaggerResponse(statusCode: 404, description: "No payment detail has been found", type: typeof(ErrorResponseDto))]
        public IActionResult DeletePaymentByPaymentId([FromRoute] Guid userId, [FromRoute] Guid paymentId)
        {
            logger.LogInformation("Received request to delete a payment detail at {0} for the user ID: {1}", paymentId, userId);

            userPaymentService.DeletePaymentByPaymentId(userId, paymentId);

            return StatusCode(StatusCodes.Status200OK);
        }

        // The below end points are specific only to inter-service communication

        /// <summary>
        /// Checks if the payment id exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <response code="200">Successfull operation</response>
        /// <response code="404">No address has been found</response>
        [HttpGet("{paymentId:guid}/verify")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, description: "Successfull operation")]
        [SwaggerResponse(statusCode: 404, description: "No payment detail has been found", type: typeof(ErrorResponseDto))]
        public IActionResult VerifyPaymentId([FromRoute] Guid userId, [FromRoute] Guid paymentId)
        {
            logger.LogInformation("Received request to verify a payment at {0} for the user ID: {1}", paymentId, userId);

            userPaymentService.PaymentIdExists(userId, paymentId);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
