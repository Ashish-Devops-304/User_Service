using Contracts.IRepository;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Repository
{
    /// <summary>
    /// Handles all the query operations for the Payment table
    /// </summary>
    public class UserPaymentRepository : IUserPaymentRepository
    {
        private readonly RepositoryContext context;
        private readonly ILogger<UserPaymentRepository> logger;

        public UserPaymentRepository(RepositoryContext context, ILogger<UserPaymentRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }


        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges()
        {
            logger.LogDebug("Saving all the changes made in the context to the DB");

            context.SaveChanges();
        }


        /// <summary>
        /// Adds a record to the payment table
        /// </summary>
        /// <param name="payment"></param>
        public void AddRecordToPayment(Payment payment)
        {
            logger.LogDebug("Adding a new record to the Payment table with the ID: " + payment.Id);

            context.Payment.Add(payment);
        }


        /// <summary>
        /// Checks if an payment information already exists for the user against name/payment value
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="paymentValue"></param>
        /// <returns></returns>
        public bool PaymentExists(Guid userId, string? name = null, string? paymentValue = null)
        {
            logger.LogDebug("Checking if the payment already exists with the name: {0} / payment value: {1} in the Payment table", name, paymentValue);

            return context.Payment.Where(a => a.UserId == userId && a.IsActive == true).Any(a => a.Name == name || a.PaymentValue == paymentValue);
        }


        /// <summary>
        /// Gets all the payment details of the user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Payment> GetAllPayment(Guid userId)
        {
            logger.LogDebug("Fetching all payment records in the Payment table for the user ID: " + userId);

            return context.Payment.Where(a => a.UserId == userId && a.IsActive == true).ToList();
        }


        /// <summary>
        /// Gets the user payment details for the given payment id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Payment GetPaymentDetails(Guid userId, Guid paymentId)
        {
            logger.LogDebug("Fetching the payment record at {0} in the Payment table", paymentId);

            return context.Payment.Where(a => a.UserId == userId && a.Id == paymentId && a.IsActive == true).SingleOrDefault();
        }

        // Specific to inter-service communication

        /// <summary>
        /// Checks if the payment id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyPaymentId(Guid userId, Guid paymentId)
        {
            logger.LogDebug("Verifying the payment record at {0} for the user ID: {1} in the Payment table", paymentId, userId);

            return context.Payment.Any(u => u.UserId == userId && u.Id == paymentId);
        }
    }
}
