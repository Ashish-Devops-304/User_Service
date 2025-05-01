using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class UserPaymentService : IUserPaymentService
    {
        private readonly IMapper mapper;
        private readonly IUserPaymentRepository userPaymentRepository;
        private readonly ICommonService commonService;
        private readonly ILogger<UserPaymentService> logger;

        public UserPaymentService(IMapper mapper, IUserPaymentRepository userPaymentRepository,
                                  ICommonService commonService, ILogger<UserPaymentService> logger)
        {
            this.mapper = mapper;
            this.userPaymentRepository = userPaymentRepository;
            this.commonService = commonService;
            this.logger = logger;
        }

        /// <summary>
        /// Stores the payment details of a customer with the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPayment"></param>
        /// <returns></returns>
        public IdentityResponseDto CreatePaymentDetail(Guid userId, PaymentCreateDto newPayment)
        {
            logger.LogDebug("Received request to add new payment detail for the user ID: " + userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            if (userPaymentRepository.PaymentExists(userId, newPayment.Name, newPayment.PaymentValue))
            {
                logger.LogError("Payment detail already exists for the user ID: {0} with the name: {1} / payment value: {2}",
                                   userId, newPayment.Name, newPayment.PaymentValue);

                throw new ConflictException("Payment detail already exists. Please try with a different name/payment value");
            }

            Payment customerPayment = mapper.Map<Payment>(newPayment);
            customerPayment.UserId = userId;

            userPaymentRepository.AddRecordToPayment(customerPayment);
            userPaymentRepository.SaveChanges();

            logger.LogDebug("New payment record added with the ID: {0} for the user ID: {1} ", customerPayment.Id, userId);

            return new IdentityResponseDto { Id = customerPayment.Id };
        }


        /// <summary>
        /// Gets all the stored payment details of a customer by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PaymentResponseDto> GetAllPaymentByUserId(Guid userId)
        {
            logger.LogDebug("Received request to get all saved payment details for the user ID: " + userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            return mapper.Map<List<PaymentResponseDto>>(userPaymentRepository.GetAllPayment(userId));
        }


        /// <summary>
        /// Get payment details of the user by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public PaymentResponseDto GetPaymentByPaymentId(Guid userId, Guid paymentId)
        {
            logger.LogDebug("Received request to get a payment detail at {0} for the user ID: {1}", paymentId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Payment userPayment = userPaymentRepository.GetPaymentDetails(userId, paymentId);

            if (userPayment == null)
            {
                logger.LogError("No payment record found at " + paymentId);

                throw new NotFoundException("No payment record has been found");
            }

            logger.LogDebug("Returned payment record at {0} for the user ID: {1}", paymentId, userId);

            return mapper.Map<PaymentResponseDto>(userPayment);
        }


        /// <summary>
        /// Update all/some properties of payment by the given payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <param name="updatedPayment"></param>
        /// <exception cref="NotFoundException"></exception>
        public void UpdatePaymentByPaymentId(Guid userId, Guid paymentId, PaymentUpdateDto updatedPayment)
        {
            logger.LogDebug("Received request to update a payment record at {0} for user ID: {1}", paymentId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Payment paymentInDB = userPaymentRepository.GetPaymentDetails(userId, paymentId);

            if (paymentInDB == null)
            {
                logger.LogError("No payment record found at " + paymentId);

                throw new NotFoundException("No payment record has been found");
            }

            bool paymentDetailExists = false;
            string propertyExists = string.Empty;

            if (updatedPayment.Name != null)
            {
                paymentDetailExists = userPaymentRepository.PaymentExists(userId, name: updatedPayment.Name);
                propertyExists = "Name";
            }

            if (updatedPayment.PaymentValue != null && !paymentDetailExists)
            {
                paymentDetailExists = userPaymentRepository.PaymentExists(userId, paymentValue: updatedPayment.PaymentValue);
                propertyExists = "Payment value";
            }

            if (paymentDetailExists)
            {
                logger.LogError(propertyExists + " already exists for user ID: " + userId);

                throw new ConflictException(propertyExists + " already exists");
            }

            var properties = typeof(PaymentUpdateDto).GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(updatedPayment);

                if (propertyValue != null)
                {
                    paymentInDB.GetType().GetProperty(propertyName).SetValue(paymentInDB, propertyValue);
                }
            }
            paymentInDB.DateUpdated = DateTime.Now;
            userPaymentRepository.SaveChanges();

            logger.LogDebug("Payment record at {0} has been updated for the user ID: {1}", paymentId, userId);
        }

        /// <summary>
        /// Delete a payment detail by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <exception cref="NotFoundException"></exception>
        public void DeletePaymentByPaymentId(Guid userId, Guid paymentId)
        {
            logger.LogDebug("Received request to delete a payment record at {0} for user ID: {1}", paymentId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Payment userPayment = userPaymentRepository.GetPaymentDetails(userId, paymentId);

            if (userPayment == null)
            {
                logger.LogError("No payment record found at " + paymentId);

                throw new NotFoundException("No payment record has been found");
            }

            userPayment.IsActive = false;
            userPaymentRepository.SaveChanges();

            logger.LogDebug("Payment record at {0} has been deleted for the user ID: {1}", paymentId, userId);
        }

        // The below methods are specific to inter-service communication

        /// <summary>
        /// Checks if the payment id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void PaymentIdExists(Guid userId, Guid paymentId)
        {
            logger.LogDebug("Received request to verify a payment record at {0} for user ID: {1}", paymentId, userId);

            if (!userPaymentRepository.VerifyPaymentId(userId, paymentId))
            {
                logger.LogError("No payment record found at {0} for user ID: {1}", paymentId, userId);

                throw new NotFoundException("No payment information has been found for the user");
            }

            logger.LogDebug("Successfully verified payment detail at {0} for the user ID: {1} ", paymentId, userId);
        }
    }
}
