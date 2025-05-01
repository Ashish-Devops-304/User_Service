using Entities.Dtos;

namespace Contracts.IServices
{
    public interface IUserAccountService
    {
        /// <summary>
        /// Create a user account
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public IdentityResponseDto CreateUserAccount(UserCreateDto userAccount);
        

        /// <summary>
        /// Authenticates the user with the given email and password and returns JWT token upon successful operation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public TokenResponseDto AuthenticateUser(UserLogInDto user);


        /// <summary>
        /// Gets the complete user account details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserAccountResponseDto GetUserAccountById(Guid userId);


        /// <summary>
        /// Admins can completely get all the user account details in the DB
        /// </summary>
        /// <returns></returns>
        public List<UserAccountOnlyResponseDto> GetAllUserAccount();


        /// <summary>
        /// Updates all/some properties of user account by the given user id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userAccount"></param>
        public void UpdateUserAccount(Guid id, UserUpdateDto userAccount);


        /// <summary>
        /// Delete a user account from DB by id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUserAccount(Guid id);


        /// <summary>
        /// Checks if the user account exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void UserIdExists(Guid userId);
    }
}
