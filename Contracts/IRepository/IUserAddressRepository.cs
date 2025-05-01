using Entities.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Contracts.IRepository
{
    public interface IUserAddressRepository
    {
        /// <summary>
        /// Saves all the changes made in the context to the DB
        /// </summary>
        public void SaveChanges();


        /// <summary>
        /// Adds a record to the address table
        /// </summary>
        /// <param name="address"></param>
        public void AddRecordToAddress(Address address);


        /// <summary>
        /// Gets all the stored delivery addresses of a customer by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Address> GetAllAddressByUserId(Guid userId);


        /// <summary>
        /// Gets the user address details for the given address id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public Address GetAddressDetails(Guid userId, Guid addressId);


        /// <summary>
        /// Checks if the user account exists by the user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool VerifyAddressId(Guid userId, Guid addressId);
    }
}
