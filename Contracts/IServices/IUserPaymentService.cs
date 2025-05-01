using Entities.Dtos;

namespace Contracts.IServices
{
    public interface IUserPaymentService
    {
        /// <summary>
        /// Stores the payment details of a customer with the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPayment"></param>
        /// <returns></returns>
        public IdentityResponseDto CreatePaymentDetail(Guid userId, PaymentCreateDto newPayment);


        /// <summary>
        /// Gets all the stored payment details of a customer by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PaymentResponseDto> GetAllPaymentByUserId(Guid userId);


        /// <summary>
        /// Get payment details of the user by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public PaymentResponseDto GetPaymentByPaymentId(Guid userId, Guid paymentId);


        /// <summary>
        /// Update all/some properties of payment by the given payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        /// <param name="updatedPayment"></param>
        public void UpdatePaymentByPaymentId(Guid userId, Guid paymentId, PaymentUpdateDto updatedPayment);


        /// <summary>
        /// Delete a payment detail by payment id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="paymentId"></param>
        public void DeletePaymentByPaymentId(Guid userId, Guid paymentId);


        /// <summary>
        /// Checks if the payment id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void PaymentIdExists(Guid userId, Guid paymentId);
    }
}
