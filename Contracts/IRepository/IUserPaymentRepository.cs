using Entities.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Contracts.IRepository
{
    public interface IUserPaymentRepository
    {
        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges();


        /// <summary>
        /// Adds a record to the payment table
        /// </summary>
        /// <param name="payment"></param>
        public void AddRecordToPayment(Payment payment);


        /// <summary>
        /// Checks if an payment information already exists for the user against name/payment value
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="paymentValue"></param>
        /// <returns></returns>
        public bool PaymentExists(Guid userId, string? name = null, string? paymentValue = null);


        /// <summary>
        /// Gets all the payment details of the user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Payment> GetAllPayment(Guid userId);


        /// <summary>
        /// Gets the user payment details for the given payment id
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public Payment GetPaymentDetails(Guid userId, Guid paymentId);


        /// <summary>
        /// Checks if the payment id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyPaymentId(Guid userId, Guid paymentId);
    }
}
