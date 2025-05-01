using Contracts.IRepository;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Repository
{
    /// <summary>
    /// Handles all the query operations for the Address table
    /// </summary>
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly RepositoryContext context;
        private readonly ILogger<UserAddressRepository> logger;

        public UserAddressRepository(RepositoryContext context, ILogger<UserAddressRepository> logger)
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
        /// Adds a record to the address table
        /// </summary>
        /// <param name="address"></param>
        public void AddRecordToAddress(Address address)
        {
            logger.LogDebug("Adding a new record to the Address table with the ID: " + address.Id);

            context.Address.Add(address);
        }


        /// <summary>
        /// Gets all the stored delivery addresses of a customer by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Address> GetAllAddressByUserId(Guid userId)
        {
            logger.LogDebug("Fetching all address records in the Address table for the user ID: " + userId);

            return context.Address.Where(a => a.UserId == userId && a.IsActive == true).ToList();
        }


        /// <summary>
        /// Gets the user address details for the given address id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Address GetAddressDetails(Guid userId, Guid addressId)
        {
            logger.LogDebug("Fetching the address record at ID: " + addressId);

            return context.Address.Where(a => a.UserId == userId && a.Id == addressId && a.IsActive == true).SingleOrDefault();
        }


        // Specific to inter-service communication

        /// <summary>
        /// Checks if the user account exists by the user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyAddressId(Guid userId, Guid addressId)
        {
            logger.LogDebug("Verifying the address record at {0} for the user ID: {1} in the Address table", addressId, userId);

            return context.Address.Any(u => u.UserId == userId && u.Id == addressId);
        }
    }
}
