using Entities.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Contracts.IRepository
{
    public interface IUserAccountRepository
    {
        /// <summary>
        /// Adds a record in the User table of context
        /// </summary>
        /// <param name="userAccount"></param>
        public void AddRecordToUser (User userAccount);


        /// <summary>
        /// Adds a record in the UserSecret table of context
        /// </summary>
        /// <param name="userSecret"></param>
        public void AddRecordToUserSecret (UserSecret userSecret);


        /// <summary>
        /// Checks if an user account already exists in the DB
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public bool AccountExists(string? email = null, long? phone = null);


        /// <summary>
        /// Gets user account id for the given email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public Guid GetUserId(string emailAddress);


        /// <summary>
        /// Gets the complete user account details for the given id
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public User GetUserDetails(Guid userId);


        /// <summary>
        /// Fetches user secret details for the given id 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public UserSecret GetUserSecretById(Guid userId);


        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges();

        
        /// <summary>
        /// Gets all the user account details in the DB
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllUserDetails();


        /// <summary>
        /// Checks if the user account exists by the user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyUserId(Guid userId);
    }
}
