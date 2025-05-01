using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Services;
using System.Security.Claims;
using UserService.Controllers;
using Xunit;

namespace UserServiceTests.Controllers
{
    public class UserAccountTests
    {
        public UserAccountController controller;

        public void SetUp(RepositoryContext context, Guid? userId = null)
        {
            ILogger<UserAccountRepository> loggerUserAccountRepo = new Mock<ILogger<UserAccountRepository>>().Object;

            IUserAccountRepository userAccountRepository = new UserAccountRepository(context, loggerUserAccountRepo);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(x => x["JWT:Key"]).Returns("mkLLpdcnji@019");
            IConfiguration iconfiguration = mockConfig.Object;

            HttpContextAccessor httpContextAccessor;

            if (userId != null)
            {
                Claim userIdClaim = new Claim("user_id", userId.ToString());
                Claim roleClaim;
                try
                {
                    roleClaim = new Claim("role", context.User.Where(u => u.Id == userId).Select(u => u.Role).SingleOrDefault());
                }

                catch (ArgumentNullException) 
                {
                    context = TestHelper.GetContextWithRecords();
                    roleClaim = new Claim("role", context.User.Where(u => u.Id == userId).Select(u => u.Role).SingleOrDefault());
                }

                ClaimsIdentity identity = new ClaimsIdentity(new[] { userIdClaim, roleClaim }, "BasicAuthentication");
                ClaimsPrincipal contextUser = new ClaimsPrincipal(identity);

                DefaultHttpContext httpContext = new DefaultHttpContext()
                {
                    User = contextUser
                };

                httpContextAccessor = new HttpContextAccessor()
                {
                    HttpContext = httpContext
                };
            }
            else
            {
                httpContextAccessor = new Mock<HttpContextAccessor>().Object;
            }

            ILogger<CommonService> loggerCommonService = new Mock<ILogger<CommonService>>().Object;

            ICommonService commonUserService = new CommonService(httpContextAccessor, loggerCommonService);

            ILogger<UserAccountService> loggerUserAccountService = new Mock<ILogger<UserAccountService>>().Object;
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            IMapper mapper = config.CreateMapper();

            IUserAccountService userAccountService = new UserAccountService(mapper, userAccountRepository, iconfiguration,
                                                                            commonUserService, loggerUserAccountService);

            ILogger<UserAccountController> loggerUserAccount = new Mock<ILogger<UserAccountController>>().Object;

            controller = new UserAccountController(userAccountService, loggerUserAccount);
        }
            
        [Fact]
        public void CreateUserAccount_WhenCalledWithValidUserAccount_Returns201WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords());

            UserCreateDto newUserAccount = new UserCreateDto
            {
                FirstName = "Siva",
                LastName = "Karthick",
                Email = "sivakarthick@gmail.com",
                Password = "JustAnotherPassword12#0",
                Phone = 9878989080,
                Role = "Customer"
            };

            IActionResult result = controller.CreateUserAccount(newUserAccount);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();
        }

        [Fact]
        public void CreateUserAccount_WhenCalledWithExistingEmailAddressInDB_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords());

            UserCreateDto newUserAccount = new UserCreateDto
            {
                FirstName = "Siva",
                LastName = "Karthick",
                Email = "iamhari@gmail.com",
                Password = "JustAnotherPassword12#0",
                Phone = 9878989080,
                Role = "Customer"
            };

            Action act = () => controller.CreateUserAccount(newUserAccount);

            act.Should().Throw<ConflictException>().WithMessage("User account already exists");
        }

        [Fact]
        public void CreateUserAccount_WhenCalledWithExistingPhoneNumberInDB_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords());

            UserCreateDto newUserAccount = new UserCreateDto
            {
                FirstName = "Siva",
                LastName = "Karthick",
                Email = "sivakarthick@gmail.com",
                Password = "JustAnotherPassword12#0",
                Phone = 9988774455,
                Role = "Customer"
            };

            Action act = () => controller.CreateUserAccount(newUserAccount);

            act.Should().Throw<ConflictException>().WithMessage("User account already exists");
        }

        [Fact]
        public void Login_WhenCalledWithValidCredentials_Returns200WithTokenResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords());

            UserLogInDto userLogIn = new UserLogInDto
            {
                Email = "iamhari@gmail.com",
                Password = "passwordPassword@12#"
            };

            IActionResult result = controller.Login(userLogIn);

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var tokenResponse = okResult.Value.Should().BeOfType<TokenResponseDto>().Subject;
            tokenResponse.AccessToken.Should().NotBeNull();
        }

        [Fact]
        public void Login_WhenCalledWithInvalidCredentials_ThrowsUnauthorizedException()
        {
            SetUp(TestHelper.GetContextWithRecords());

            UserLogInDto userLogIn = new UserLogInDto
            {
                Email = "iamhari@gmail.com",
                Password = "wrongpassword"
            };

            Action act = () => controller.Login(userLogIn);

            act.Should().Throw<UnauthorizedException>().WithMessage("Email/password is incorrect");

        }

        [Fact]
        public void GetUserAccountById_WhenCalledWithValidUserIdInDBAndClaims_Returns200WithUserAccountDetails()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new UserAccountResponseDto
            {
                Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                FirstName = "Sudharsan",
                LastName = "S",
                Email = "sudharsans@gmail.com",
                Phone = 6324789563,
                Role = "Customer",
                Addresses = new List<AddressResponseDto>
                {
                    new AddressResponseDto
                    {
                        Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                        Name = "Sudharsan",
                        Type = "Home",
                        Phone = 9666993344,
                        Line1 = "9, Green Park Layout",
                        Line2 = "2nd street",
                        City = "Chennai",
                        Pincode = 600116,
                        State = "Tamil Nadu",
                        Country = "India"                    
                    }
                },
                
                Payments = new List<PaymentResponseDto>
                { 
                    new PaymentResponseDto
                    {
                        Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                        Type = "UPI",
                        Name = "GPay",
                        PaymentValue = "mygpayupi@paytm"
                    }
                }
            });
        }

        [Fact]
        public void GetUserAccountById_WhenTheUserIsACustomerAndRequestingAccountDetailOfOtherUser_ThrowsForbiddenException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetUserAccountById(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<ForbiddenException>().WithMessage("Access to this resource is denied");

        }

        [Fact]
        public void GetUserAccountById_WhenAUserIsRequestingAccountDetailOfOtherUserAccountThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetUserAccountById(Guid.Parse("12304722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");

        }

        [Fact]
        public void GetUserAccountById_WhenTheUserIsAnAdminAndRequestingAccountDetailOfOtherUser_Returns200WithUserAccountDetails()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new UserAccountResponseDto
            {
                Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                FirstName = "Sudharsan",
                LastName = "S",
                Email = "sudharsans@gmail.com",
                Phone = 6324789563,
                Role = "Customer",
                Addresses = new List<AddressResponseDto>
                {
                    new AddressResponseDto
                    {
                        Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                        Name = "Sudharsan",
                        Type = "Home",
                        Phone = 9666993344,
                        Line1 = "9, Green Park Layout",
                        Line2 = "2nd street",
                        City = "Chennai",
                        Pincode = 600116,
                        State = "Tamil Nadu",
                        Country = "India"
                    }
                },

                Payments = new List<PaymentResponseDto>
                {
                    new PaymentResponseDto
                    {
                        Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                        Type = "UPI",
                        Name = "GPay",
                        PaymentValue = "mygpayupi@paytm"
                    }
                }
            });
        }

        [Fact]
        public void GetAllUserAccount_WhenCalledByAnAdminAndIfThereIsAtLeastOneRecord_Retruns200WithListOfUserAccountResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetAllUserAccount();

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new List<UserAccountOnlyResponseDto>
            {
                new UserAccountOnlyResponseDto
                {
                    Id = Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"),
                    FirstName = "Hariharan",
                    LastName = "A S",
                    Email = "iamhari@gmail.com",
                    Phone = 9988774455,
                    Role = "Admin"
                },

                new UserAccountOnlyResponseDto
                {
                    Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                    FirstName = "Sudharsan",
                    LastName = "S",
                    Email = "sudharsans@gmail.com",
                    Phone = 6324789563,
                    Role = "Customer"                
                },

                new UserAccountOnlyResponseDto
                {
                    Id = Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"),
                    FirstName = "Surya",
                    LastName = "Kumar",
                    Email = "suryakumar@gmail.com",
                    Phone = 9874521036,
                    Role = "Customer"
                }
            });
        }

        [Fact]
        public void GetAllUserAccount_WhenCalledByAnAdminAndIfThereIsNoRecord_Retruns204()
        {
            SetUp(TestHelper.GetEmptyContext(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetAllUserAccount();

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        //[Fact]
        public void GetAllUserAccount_WhenCalledByACustomer_ThrowsForbiddenException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetAllUserAccount();

            act.Should().Throw<ForbiddenException>().WithMessage("Access to this resource is denied");

        }

        [Fact]
        public void UpdateUserAccountById_WhenCalledByACustomerToUpdateOwnAccountWithValidValues_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            UserUpdateDto updatedUserAccount = new UserUpdateDto
            {
                LastName = "S K",
                Email = "sivakarthick@gmail.com",
                Password = "JustAnotherPassword12#",
                Phone = 9878989080
            };

            IActionResult resultUpdateUserAccount = controller.UpdateUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), updatedUserAccount);

            IActionResult resultGetUserAccount = controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            StatusCodeResult okResultUpdate = resultUpdateUserAccount.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUserAccount.Should().BeOfType<ObjectResult>().Subject;
            okResultGet.Value.Should().BeEquivalentTo(new UserAccountResponseDto
            {
                Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                FirstName = "Sudharsan",
                LastName = "S K",
                Email = "sivakarthick@gmail.com",
                Phone = 9878989080,
                Role = "Customer",
                Addresses = new List<AddressResponseDto>
                {
                    new AddressResponseDto
                    {
                        Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                        Name = "Sudharsan",
                        Type = "Home",
                        Phone = 9666993344,
                        Line1 = "9, Green Park Layout",
                        Line2 = "2nd street",
                        City = "Chennai",
                        Pincode = 600116,
                        State = "Tamil Nadu",
                        Country = "India"
                    }
                },
                Payments = new List<PaymentResponseDto>
                {
                    new PaymentResponseDto
                    {
                        Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                        Type = "UPI",
                        Name = "GPay",
                        PaymentValue = "mygpayupi@paytm"
                    }
                }
            });
        }

        [Fact]
        public void UpdateUserAccountById_WhenCalledByCustomerToUpdateOtherUserAccount_ThrowsForbiddenException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            UserUpdateDto updatedUserAccount = new UserUpdateDto
            {
                LastName = "S K",
                Email = "sivakarthick@gmail.com",
                Password = "JustAnotherPassword12#",
                Phone = 9878989080
            };

            Action act = () => controller.UpdateUserAccountById(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), updatedUserAccount);

            act.Should().Throw<ForbiddenException>().WithMessage("Access to this resource is denied");
        }

        [Fact]
        public void UpdateUserAccountById_WhenCalledByAnAdminWithOtherUserAccount_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            UserUpdateDto updatedUserAccount = new UserUpdateDto
            {
                LastName = "S K",
                Email = "sivakarthick@gmail.com",
                Phone = 9878989080
            };

            IActionResult resultUpdateUserAccount = controller.UpdateUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), updatedUserAccount);

            IActionResult resultGetUserAccount = controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            StatusCodeResult okResultUpdate = resultUpdateUserAccount.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUserAccount.Should().BeOfType<ObjectResult>().Subject;
            okResultGet.Value.Should().BeEquivalentTo(new UserAccountResponseDto
            {
                Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                FirstName = "Sudharsan",
                LastName = "S K",
                Email = "sivakarthick@gmail.com",
                Phone = 9878989080,
                Role = "Customer",
                Addresses = new List<AddressResponseDto>
                {
                    new AddressResponseDto
                    {
                        Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                        Name = "Sudharsan",
                        Type = "Home",
                        Phone = 9666993344,
                        Line1 = "9, Green Park Layout",
                        Line2 = "2nd street",
                        City = "Chennai",
                        Pincode = 600116,
                        State = "Tamil Nadu",
                        Country = "India"
                    }
                },
                Payments = new List<PaymentResponseDto>
                {
                    new PaymentResponseDto
                    {
                        Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                        Type = "UPI",
                        Name = "GPay",
                        PaymentValue = "mygpayupi@paytm"
                    }
                }
            });
        }

        [Fact]
        public void UpdateUserAccountById_WhenUpdatedWithExistingEmailOrPhone_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            UserUpdateDto updatedUserAccount = new UserUpdateDto
            {
                Email = "iamhari@gmail.com",
                Phone = 9878989080
            };

            Action act = () => controller.UpdateUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), updatedUserAccount);

            act.Should().Throw<ConflictException>().WithMessage("Email/phone already exists");
        }

        [Fact]
        public void UpdateUserAccountById_WhenCalledToUpdateAnAccountThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            UserUpdateDto updatedUserAccount = new UserUpdateDto
            {
                Email = "sivakarthick@gmail.com",
                Password = "JustAnotherPassword12#@",
                Phone = 9878989080,
                Role = "Customer"
            };

            Action act = () => controller.UpdateUserAccountById(Guid.Parse("12342104-c943-4716-aef0-feeea5ac8cce"), updatedUserAccount);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void DeleteUserAccountById_WhenCalledByCustomerToDeleteAccount_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult resultDelete = controller.DeleteUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void DeleteUserAccountById_WhenCalledByCustomerToDeleteOtherUserAccount_ThrowsForbiddenException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeleteUserAccountById(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<ForbiddenException>().WithMessage("Access to this resource is denied");
        }

        [Fact]
        public void DeleteUserAccountById_WhenCalledByAdminToDeleteOtherUserAccount_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult resultDelete = controller.DeleteUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetUserAccountById(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void DeleteUserAccountById_WhenCalledByUserToDeleteUserAccountThatDoesNotExists_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeleteUserAccountById(Guid.Parse("12304722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void VerifyUserId_WhenCalledWithExistentUserIdInDB_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.VerifyUserId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        }

        [Fact]
        public void VerifyUserId_WhenCalledWithNonExistentUserIdInDB_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.VerifyUserId(Guid.Parse("12332104-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }
    }
}
