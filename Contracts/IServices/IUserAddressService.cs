using Entities.Dtos;

namespace Contracts.IServices
{
    public interface IUserAddressService
    {

        /// <summary>
        /// Stores the delivery address details of a customer with the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateAddress(Guid userId, AddressCreateDto address);


        /// <summary>
        /// Gets all the stored delivery addresses of a customer by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<AddressResponseDto> GetAllAddressByUserId(Guid userId);


        /// <summary>
        /// Get address details of the user by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AddressResponseDto GetAddressByAddressId(Guid userId, Guid addressId);


        /// <summary>
        /// Update all/some properties of address by the given address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <param name="updatedAddress"></param>
        public void UpdateAddressByAddressId(Guid userId, Guid addressId, AddressUpdateDto updatedAddress);


        /// <summary>
        /// Delete an address by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        public void DeleteAddressByAddressId(Guid userId, Guid addressId);


        /// <summary>
        /// Checks if the address id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void AddressIdExists(Guid userId, Guid addressId);
    }
}
