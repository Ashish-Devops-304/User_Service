using Contracts.IRepository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository
{
    /// <summary>
    /// Handle all the query operations for the User and UserSecret tables
    /// </summary>
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly RepositoryContext context;
        private readonly ILogger<UserAccountRepository> logger;

        public UserAccountRepository(RepositoryContext context, ILogger<UserAccountRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a record in the User table of context
        /// </summary>
        /// <param name="userAccount"></param>
        public void AddRecordToUser(User userAccount)
        {
            logger.LogDebug("Adding a new record to the User table with the ID: " + userAccount.Id);
            context.User.Add(userAccount);            
        }


        /// <summary>
        /// Adds a record in the UserSecret table of context
        /// </summary>
        /// <param name="userAccount"></param>
        public void AddRecordToUserSecret(UserSecret userSecret)
        {
            logger.LogDebug("Adding a new record to the User Secret table with the ID: " + userSecret.Id);

            context.UserSecret.Add(userSecret);            
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
        /// Checks if an user account already exists in the DB
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public bool AccountExists(string? email = null, long? phone = null)
        {
            logger.LogDebug("Checking if a user account exists in the User table");

            return context.User.Any( a => (a.Email == email || a.Phone == phone) && a.IsActive == true);
        }


        /// <summary>
        /// Gets user account id for the given email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public Guid GetUserId(string emailAddress)
        {
            logger.LogDebug("Getting user id in the User table for the email address: " + emailAddress);

            return context.User.Where(a => a.Email == emailAddress && a.IsActive == true).Select(a => a.Id).SingleOrDefault();
        }


        /// <summary>
        /// Gets the complete user account details for the given id
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public User GetUserDetails(Guid userId)
        {
            logger.LogDebug("Fetching complete user account details for user ID: " + userId);

            return context.User
                   .Include(u => u.Address.Where(a => a.IsActive == true))
                   .Include(u => u.Payment.Where(p => p.IsActive == true))
                   .FirstOrDefault(u => u.Id == userId && u.IsActive == true);
        }


        /// <summary>
        /// Fetches user secret details for the given id
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public UserSecret GetUserSecretById(Guid userId)
        {
            logger.LogDebug("Fetching user account password in the User Secret table for user ID: " + userId);

            return context.UserSecret.Where(a => a.UserId == userId && a.IsActive == true).SingleOrDefault();
        }

        /// <summary>
        /// Gets all the user account details in the DB
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<User> GetAllUserDetails()
        {
            logger.LogDebug("Fetching all user account details in the User table");

            return context.User.Where(a => a.IsActive == true).ToList();
        }

        // Specific to inter-service communication

        /// <summary>
        /// Checks if the user account exists by the user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public bool VerifyUserId(Guid userId)
        {
            logger.LogDebug("Verifying the user ID: {0} in the User table", userId);

            return context.User.Any(u => u.Id == userId && u.IsActive == true);
        }
    }
}
