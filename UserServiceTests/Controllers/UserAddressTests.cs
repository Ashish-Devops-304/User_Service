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
    public class UserAddressTests
    {
        public UserAddressController controller;

        public void SetUp(RepositoryContext context, Guid userId)
        {
            ILogger<UserAddressRepository> loggerUserAddressRepo = new Mock<ILogger<UserAddressRepository>>().Object;

            IUserAddressRepository userAddressRepo = new UserAddressRepository(context, loggerUserAddressRepo);

            Claim userIdClaim = new Claim("user_id", userId.ToString());
            Claim roleClaim = new Claim("role", context.User.Where(u => u.Id == userId).Select(u => u.Role).SingleOrDefault());
            ClaimsIdentity identity = new ClaimsIdentity(new[] { userIdClaim, roleClaim }, "BasicAuthentication");
            
            ClaimsPrincipal contextUser = new ClaimsPrincipal(identity);

            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            HttpContextAccessor httpContextAccessor = new HttpContextAccessor()
            {
                HttpContext = httpContext
            };

            ILogger<CommonService> loggerCommonService = new Mock<ILogger<CommonService>>().Object;

            ICommonService commonUserService = new CommonService(httpContextAccessor, loggerCommonService);

            ILogger<UserAddressService> loggerUserAddressService = new Mock<ILogger<UserAddressService>>().Object;
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            IMapper mapper = config.CreateMapper();


            ILogger<UserAccountRepository> loggerUserAccountRepo = new Mock<ILogger<UserAccountRepository>>().Object;
            IUserAccountRepository userAccountRepository = new UserAccountRepository(context, loggerUserAccountRepo);
            IUserAccountService userAccountService = new UserAccountService(new Mock<IMapper>().Object, userAccountRepository, new Mock<IConfiguration>().Object,
                                                                            new Mock<ICommonService>().Object, new Mock<ILogger<UserAccountService>>().Object);


            IUserAddressService userAddressService = new UserAddressService(mapper, userAddressRepo, commonUserService,
                                                                            loggerUserAddressService, userAccountService);

            ILogger<UserAddressController> loggerUserAddressController = new Mock<ILogger<UserAddressController>>().Object;

            controller = new UserAddressController(userAddressService, loggerUserAddressController);
        }


        [Fact]
        public void CreateAddress_WhenCreatedWithValidAddressAndUserId_Returns201StatusWithIdentityResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddressCreateDto address = new AddressCreateDto
            {
                Name = "Rithikesh",
                Type = "Work",
                Phone = 8877445566,
                Line1 = "4/576B",
                Line2 = "BMC Nagar",
                City = "Coimbatore",
                Pincode = 642126,
                State = "Tamil Nadu",
                Country = "India"
            };

            IActionResult result = controller.CreateAddress(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), address);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();
        }

        [Fact]
        public void CreateAddress_WhenCreatedByCustomerWithValidAddressToOtherUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddressCreateDto address = new AddressCreateDto
            {
                Name = "Rithikesh",
                Type = "Home",
                Phone = 8877445566,
                Line1 = "4/576B",
                Line2 = "BMC Nagar",
                City = "Coimbatore",
                Pincode = 642126,
                State = "Tamil Nadu",
                Country = "India"
            };

            Action act = () => controller.CreateAddress(Guid.Parse("12323014-c943-4716-aef0-feeea5ac8cce"), address);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        
        [Fact]
        public void GetAllUserAddress_WhenCalledWithValidUserIdWithAtLeastOneRecord_Retruns200WithListOfAddresses()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllAddressByUserId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(new List<AddressResponseDto>()
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
            });
        }

        [Fact]
        public void GetAllUserAddress_WhenCalledByAdminToAccessOtherUserAccountAddressesWithAtLeastOneRecord_Retruns200WithListOfAddresses()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllAddressByUserId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(new List<AddressResponseDto>()
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
            });
        }

        [Fact]
        public void GetAllUserAddress_WhenCalledByCustomerToAccessOtherUserAccountAddressesWithAtLeastOneRecord_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetAllAddressByUserId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetAllUserAddress_WhenCalledByUserToGetAddressesOfUserWithNoRecords_Returns204()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            IActionResult result = controller.GetAllAddressByUserId(Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetAddressByAddressId_WhenCalledByValidUser_Returns200WithAddressAtRequestedId()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new AddressResponseDto
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
            });
        }

        [Fact]
        public void GetAddressByAddressId_WhenUserAccessesTheAddressIdOfOtherUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            Action act = () => controller.GetAddressByAddressId(Guid.Parse("1230cbb1-b299-4314-8b75-b6a9f5d7f014"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetAddressByAddressId_WhenCalledByUserWhoDoesNotHaveRequestedAddressId_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            Action act = () => controller.GetAddressByAddressId(Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));
            
            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the given id");
        }

        [Fact]
        public void GetAddressByAddressId_WhenCalledByAdminToAccessOtherUserAccountAddress_Returns200WithAddressAtRequestedId()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new AddressResponseDto
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
            });
        }

        [Fact]
        public void UpdateAddressByAddressId_WhenCalledByACustomerToUpdateOwnAddressWithValidValues_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddressUpdateDto updatedAddress = new AddressUpdateDto
            {
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566
            };

            IActionResult updatedResult = controller.UpdateAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"), updatedAddress);

            IActionResult resultGetUserAddress = controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResultUpdate = updatedResult.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUserAddress.Should().BeOfType<ObjectResult>().Subject;

            okResultGet.Value.Should().BeEquivalentTo(new AddressResponseDto
            {
                Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566,
                Line1 = "9, Green Park Layout",
                Line2 = "2nd street",
                City = "Chennai",
                Pincode = 600116,
                State = "Tamil Nadu",
                Country = "India"
            });
        }


        [Fact]
        public void UpdateAddressByAddressId_WhenCalledByACustomerToUpdateOtherAccountAddressWithValidValues_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddressUpdateDto updatedAddress = new AddressUpdateDto
            {
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566
            };

            Action act = () => controller.UpdateAddressByAddressId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), Guid.Parse("5c9f4c63-f510-4ae1-95c1-4e5a43c42c07"), updatedAddress);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void UpdateAddressByAddressId_WhenAddressIdDoesNotExistsForTheUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            AddressUpdateDto updatedAddress = new AddressUpdateDto
            {
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566
            };

            Action act = () => controller.UpdateAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("5c9f4c63-f510-4ae1-95c1-4e5a43c42c07"), updatedAddress);

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found");
        }

        [Fact]
        public void UpdateAddressByAddressId_WhenCalledByAdminToUpdateOtherAccountAddressWithValidValues_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            AddressUpdateDto updatedAddress = new AddressUpdateDto
            {
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566
            };

            IActionResult updatedResult = controller.UpdateAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"), updatedAddress);

            IActionResult resultGetUserAddress = controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResultUpdate = updatedResult.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUserAddress.Should().BeOfType<ObjectResult>().Subject;

            okResultGet.Value.Should().BeEquivalentTo(new AddressResponseDto
            {
                Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                Name = "Rithwin",
                Type = "Home",
                Phone = 8877445566,
                Line1 = "9, Green Park Layout",
                Line2 = "2nd street",
                City = "Chennai",
                Pincode = 600116,
                State = "Tamil Nadu",
                Country = "India"
            });
        }

        [Fact]
        public void DeleteAddressByAddressId_WhenCalledByUserToDeleteOwnAddress_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult resultDelete = controller.DeleteAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            Action act = () => controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the given Id");
        }

        [Fact]
        public void DeleteAddressByAddressId_WhenCalledByUserToDeleteOtherUserAddress_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeleteAddressByAddressId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), Guid.Parse("5c9f4c63-f510-4ae1-95c1-4e5a43c42c07"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void DeleteAddressByAddressId_WhenCalledByAdminToDeleteOtherUserAddress_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult resultDelete = controller.DeleteAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            Action act = () => controller.GetAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the given Id");
        }

        [Fact]
        public void DeleteAddressByAddressId_WhenCalledByUserToDeleteAddressThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeleteAddressByAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("5c9f4c63-f510-4ae1-95c1-4e5a43c42c07"));

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the given id");
        }

        [Fact]
        public void VerifyAddressId_WhenAddressIdExistsForTheUser_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.VerifyAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public void VerifyAddressId_WhenAddressIdDoesNotExistsForTheUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.VerifyAddressId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("12332104-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No address has been found for the user");
        }
    }
}
