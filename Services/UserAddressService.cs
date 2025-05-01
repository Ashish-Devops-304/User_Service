using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IMapper mapper;
        private readonly IUserAddressRepository userAddressRepository;
        private readonly ICommonService commonService;
        private readonly ILogger<UserAddressService> logger;
        private readonly IUserAccountService userAccountService;

        public UserAddressService(IMapper mapper, IUserAddressRepository userAddressRepository, ICommonService commonService, 
                                  ILogger<UserAddressService> logger, IUserAccountService userAccountService)
        {
            this.mapper = mapper;
            this.userAddressRepository = userAddressRepository;
            this.commonService = commonService;
            this.logger = logger;
            this.userAccountService = userAccountService;
        }

        /// <summary>
        /// Stores the delivery address details of a customer with the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateAddress(Guid userId, AddressCreateDto address)
        {
            logger.LogDebug("Received request to add an address for the user ID: " + userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Address customerAddress = mapper.Map<Address>(address);
            customerAddress.UserId = userId;

            userAddressRepository.AddRecordToAddress(customerAddress);
            userAddressRepository.SaveChanges();

            logger.LogDebug("New address added with the ID: {0} for the user ID: {1} ", customerAddress.Id, userId);

            return new IdentityResponseDto { Id = customerAddress.Id };
        }


        /// <summary>
        /// Gets all the stored delivery addresses of a customer by user id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public List<AddressResponseDto> GetAllAddressByUserId(Guid userId)
        {
            logger.LogDebug("Received request to get all saved addresses for the user ID: " + userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            return mapper.Map<List<AddressResponseDto>>(userAddressRepository.GetAllAddressByUserId(userId));
        }


        /// <summary>
        /// Get address details of the user by address id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public AddressResponseDto GetAddressByAddressId(Guid userId, Guid addressId)
        {
            logger.LogDebug("Received request to get an address at {0} for user ID: {1}", addressId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Address userAddress = userAddressRepository.GetAddressDetails(userId, addressId);

            if (userAddress == null)
            {
                logger.LogError("No address found at {0} for the user Id: {1}", addressId, userId);

                throw new NotFoundException("No address has been found for the given Id");
            }

            logger.LogDebug("Returned address record at {0} for the user ID: {1}", addressId, userId);

            return mapper.Map<AddressResponseDto>(userAddress);    
        }


        /// <summary>
        /// Update all/some properties of address by the given address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        /// <param name="updatedAddress"></param>
        /// <exception cref="NotFoundException"></exception>
        public void UpdateAddressByAddressId(Guid userId, Guid addressId, AddressUpdateDto updatedAddress)
        {
            logger.LogDebug("Received request to update an address at {0} for user ID: {1}", addressId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Address addressInDB = userAddressRepository.GetAddressDetails(userId, addressId);

            if (addressInDB == null)
            {
                logger.LogError("No address found at {0} for user ID: {1}", addressId, userId);

                throw new NotFoundException("No address has been found");
            }

            var properties = typeof(AddressUpdateDto).GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(updatedAddress);

                if (propertyValue != null)
                {
                    addressInDB.GetType().GetProperty(propertyName).SetValue(addressInDB, propertyValue);
                }
            }

            addressInDB.DateUpdated = DateTime.Now;
            userAddressRepository.SaveChanges();

            logger.LogDebug("Address at {0} has been updated for the user ID: {1}", addressId, userId);
        }


        /// <summary>
        /// Delete an address by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        public void DeleteAddressByAddressId(Guid userId, Guid addressId)
        {
            logger.LogDebug("Received request to delete an address at {0} for user ID: {1}", addressId, userId);

            commonService.UserIdAccessCheckForAddressOrPayment(userId);

            Address addressInDB = userAddressRepository.GetAddressDetails(userId, addressId);

            if (addressInDB == null)
            {
                logger.LogError("No address found at " + addressId);

                throw new NotFoundException("No address has been found for the given Id");
            }

            addressInDB.IsActive = false;
            userAddressRepository.SaveChanges();

            logger.LogDebug("Address at {0} has been deleted for the user ID: {1}", addressId, userId);
        }


        // The below methods are specific to inter-service communication

        /// <summary>
        /// Checks if the address id exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void AddressIdExists(Guid userId, Guid addressId)
        {
            logger.LogDebug("Received request to verify an address at {0} for the user ID: {1}", addressId, userId);

            if (!userAddressRepository.VerifyAddressId(userId, addressId))
            {
                logger.LogError("No address found at {0} for user ID: {1}", addressId, userId);

                throw new NotFoundException("No address has been found for the user");
            }

            logger.LogDebug("Successfully verified address at {0} for the user ID: {1} ", addressId, userId);
        }
    }
}
