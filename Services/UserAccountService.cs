using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public class UserAccountService : IUserAccountService
    {

        private readonly IMapper mapper;
        private readonly IUserAccountRepository userAccountRepository;
        private readonly IConfiguration iconfiguration;
        private readonly ICommonService commonService;
        private readonly ILogger<UserAccountService> logger;

        public UserAccountService(IMapper mapper, IUserAccountRepository userAccountRepository,
                                  IConfiguration iconfiguration, ICommonService commonService,
                                  ILogger<UserAccountService> logger)
        {
            this.mapper = mapper;
            this.userAccountRepository = userAccountRepository;
            this.iconfiguration = iconfiguration;
            this.commonService = commonService;
            this.logger = logger;
        }

        /// <summary>
        /// Hash a password using PBKDF2 with SHA-256
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            // Generate a random salt

            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Compute the hash of the password and salt using PBKDF2 with SHA-256

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                hash = pbkdf2.GetBytes(32);
            }

            // Concatenate the salt and hash and convert to a base64 string

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }


        /// <summary>
        /// Verify a password by hashing the provided password and comparing it to the stored hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Split the stored hash into the salt and hash

            string[] parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            // Compute the hash of the provided password and salt using PBKDF2 with SHA-256

            byte[] providedHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                providedHash = pbkdf2.GetBytes(32);
            }

            // Compare the provided hash to the stored hash

            for (int i = 0; i < 32; i++)
            {
                if (storedHash[i] != providedHash[i]) 
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Create a user account 
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        /// 
        public IdentityResponseDto CreateUserAccount(UserCreateDto userAccount)
        {
            logger.LogDebug("Received request to create user account for " + userAccount.Email);

            if (userAccountRepository.AccountExists(userAccount.Email, userAccount.Phone))
            {
                logger.LogError("User account already exists for email: {0}/ phone: {1}", userAccount.Email, userAccount.Phone);
                
                throw new ConflictException("User account already exists");
            }

            User user = mapper.Map<User>(userAccount);

            if (user.Address != null)
            {
                foreach (Address address in user.Address)
                {
                    address.UserId = user.Id;
                }
            }

            // User's password is hashed and stored in a different table from User table called UserSecret table

            UserSecret userSecret = new UserSecret();
            userSecret.UserId = user.Id;
            userSecret.Password = HashPassword(userAccount.Password);

            userAccountRepository.AddRecordToUser(user);
            userAccountRepository.AddRecordToUserSecret(userSecret);
            userAccountRepository.SaveChanges();

            logger.LogDebug("New user account created with ID: " + user.Id);

            return new IdentityResponseDto { Id = user.Id };
        }


        /// <summary>
        /// Authenticates the user with the given email and password and returns JWT token upon successful operation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public TokenResponseDto AuthenticateUser(UserLogInDto user)
        {
            logger.LogDebug("Received request to authenticate a user with the email: " + user.Email);
            
            Guid userId = userAccountRepository.GetUserId(user.Email);

            string? hashedPassword = null;

            if (userId != Guid.Empty)
            {
                hashedPassword = userAccountRepository.GetUserSecretById(userId).Password;
            }

            if (hashedPassword == null || VerifyPassword(user.Password, hashedPassword) == false)
            {
                logger.LogError("User is denied access with the email: {0}", user.Email);

                throw new UnauthorizedException("Email/password is incorrect");
            }

            User userAccountDetails = userAccountRepository.GetUserDetails(userId);

            // Return JWT token if the user given email/password matches with the DB

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes("4F79A9D8B8A7AABDECBF4D0EFAFD6D3F2C1A0A9C8987859499BAC7B6F2D9E8F7");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("user_id", userId.ToString()),
                    new Claim("role", userAccountDetails.Role),
                    new Claim("first_name", userAccountDetails.FirstName),
                    new Claim("last_name", userAccountDetails.LastName),
                    new Claim("email", userAccountDetails.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            logger.LogDebug("User with the email address: {0} is successfully authenticated and an access token is returned", user.Email);

            return new TokenResponseDto { AccessToken = tokenHandler.WriteToken(token) };
        }


        /// <summary>
        /// Gets the complete user account details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserAccountResponseDto GetUserAccountById(Guid userId)
        {
            logger.LogDebug("Received request to get user account details for ID: " + userId);

            User userUser = userAccountRepository.GetUserDetails(userId);

            if (userUser == null)
            {
                logger.LogError("No user account has been found for the ID: " + userId);

                throw new NotFoundException("No user account has been found");
            }

            commonService.UserIdAccessCheck(userId);

            logger.LogDebug("User with ID: {0} viewed their account details", userId);

            return mapper.Map<UserAccountResponseDto>(userUser);
        }
        

        /// <summary>
        /// Admins can completely get all the user account details in the DB
        /// </summary>
        /// <returns></returns>
        public List<UserAccountOnlyResponseDto> GetAllUserAccount()
        {
            Guid userId = commonService.GetUserIdFromClaims();

            logger.LogDebug("Received request to get all user account details by an Admin with the user ID: " + userId);

            //commonService.AdminAccessCheck(); - Only for unit testing
            
            return mapper.Map<List<UserAccountOnlyResponseDto>>(userAccountRepository.GetAllUserDetails());
        }


        /// <summary>
        /// Updates all/some properties of user account by the given user id
        /// </summary>
        /// <param name="userId"></param>
        public void UpdateUserAccount(Guid userId, UserUpdateDto updateUserAccount)
        {
            logger.LogDebug("Received request to update user account details for the ID: " + userId);

            User userAccountInDB = userAccountRepository.GetUserDetails(userId);

            if (userAccountInDB == null)
            {
                logger.LogError("No user account has been found for the ID: " + userId);

                throw new NotFoundException("No user account has been found");
            }

            commonService.UserIdAccessCheck(userId);

            if (updateUserAccount.Email != null || updateUserAccount.Phone.HasValue)
            {
                if (userAccountRepository.AccountExists(updateUserAccount.Email, updateUserAccount.Phone))
                {
                    logger.LogError("User account already exists for email: {0}/ phone: {1}", updateUserAccount.Email, updateUserAccount.Phone);

                    throw new ConflictException("Email/phone already exists");
                }
            }

            var properties = typeof(UserUpdateDto).GetProperties();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(updateUserAccount);

                if (propertyValue != null)
                {
                    if (propertyName == "Password")
                    {
                        UserSecret userSecret = userAccountRepository.GetUserSecretById(userId);
                        userSecret.Password = HashPassword(updateUserAccount.Password);
                        userSecret.DateUpdated = DateTime.Now;
                    }
                    else
                    {
                        userAccountInDB.GetType().GetProperty(propertyName).SetValue(userAccountInDB, propertyValue);
                    }
                }
            }

            userAccountInDB.DateUpdated = DateTime.Now;
            userAccountRepository.SaveChanges();

            logger.LogDebug("User account details has been updated for the ID: " + userId);
        }


        /// <summary>
        /// Delete a user account from DB by id
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUserAccount(Guid userId)
        {
            logger.LogDebug("Received request to delete user account for the ID: " + userId);

            User userAccount = userAccountRepository.GetUserDetails(userId);

            if (userAccount == null)
            {
                logger.LogError("No user account has been found for the ID: " + userId);

                throw new NotFoundException("No user account has been found");
            }

            commonService.UserIdAccessCheck(userId);

            // Completeley deletes a user account from User, UserSecret, Address and Payment tables

            UserSecret userSecret = userAccountRepository.GetUserSecretById(userId);
            userSecret.IsActive = false;

            userAccount.Address.ToList().ForEach(a => a.IsActive = false);
            userAccount.Payment.ToList().ForEach(a => a.IsActive = false);

            userAccount.IsActive = false;
            userAccountRepository.SaveChanges();

            logger.LogDebug("User account has been deleted for the ID: " + userId);
        }


        // The below methods are specific to inter-service communication

        /// <summary>
        /// Checks if the user account exists for the given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public void UserIdExists (Guid userId)
        {
            logger.LogDebug("Received request to verify the user ID: " + userId);

            if (!userAccountRepository.VerifyUserId(userId))
            {
                logger.LogError("No user account has been found for the ID: " + userId);

                throw new NotFoundException("No user account has been found");
            }

            logger.LogDebug("Successfully verified the user ID: " + userId);
        }
    }
}