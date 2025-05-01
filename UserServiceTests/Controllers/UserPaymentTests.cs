using AutoMapper;
using Contracts.IRepository;
using Contracts.IServices;
using Entities.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Services;
using System.Security.Claims;
using UserService.Controllers;
using Xunit;

namespace UserServiceTests.Controllers
{
    public class UserPaymentTests
    {
        public UserPaymentController controller;

        public void SetUp(RepositoryContext context, Guid userId)
        {
            ILogger<UserPaymentRepository> loggerUserPaymentRepo = new Mock<ILogger<UserPaymentRepository>>().Object;

            IUserPaymentRepository userPaymentRepo = new UserPaymentRepository(context, loggerUserPaymentRepo);

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

            ILogger<UserPaymentService> loggerUserAddressService = new Mock<ILogger<UserPaymentService>>().Object;
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            IMapper mapper = config.CreateMapper();

            IUserPaymentService userPaymentService = new UserPaymentService(mapper, userPaymentRepo, commonUserService,
                                                                            loggerUserAddressService);

            ILogger<UserPaymentController> loggerUserPaymentController = new Mock<ILogger<UserPaymentController>>().Object;

            controller = new UserPaymentController(userPaymentService, loggerUserPaymentController);
        }

        [Fact]
        public void CreatePaymentDetail_WhenCreatedWithValidPaymentInformation_Returns201WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentCreateDto newPayment = new PaymentCreateDto
            {
                Type = "UPI",
                Name = "UPI Personal",
                PaymentValue = "mynewgpayupi@paytm"
            };

            IActionResult result = controller.CreatePaymentDetail(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), newPayment);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();
        }

        [Fact]
        public void CreatePaymentDetail_WhenCustomerIsCreatingAPaymentInformationForOtherUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentCreateDto newPayment = new PaymentCreateDto
            {
                Type = "UPI",
                Name = "UPI Personal",
                PaymentValue = "mynewgpayupi@paytm"
            };

            Action act = () => controller.CreatePaymentDetail(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), newPayment);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void CreatePaymentDetail_WhenAdminIsCreatingPaymentInformationForOtherUser_Returns201WithIdentityResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            PaymentCreateDto newPayment = new PaymentCreateDto
            {
                Type = "UPI",
                Name = "UPI Personal",
                PaymentValue = "mynewgpayupi@paytm"
            };

            IActionResult result = controller.CreatePaymentDetail(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), newPayment);

            ObjectResult createdResult = result.Should().BeOfType<ObjectResult>().Subject;
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().BeOfType<IdentityResponseDto>();
        }

        [Fact]
        public void CreatePaymentDetail_WhenCreatedWithExistingPaymentValueOrName_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentCreateDto newPayment = new PaymentCreateDto
            {
                Type = "UPI",
                Name = "GPay",
                PaymentValue = "mygpayupi@paytm"
            };

            Action act = () => controller.CreatePaymentDetail(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), newPayment);

            act.Should().Throw<ConflictException>().WithMessage("Payment detail already exists. Please try with a different name/payment value");
        }

        [Fact]
        public void GetAllPaymentByUserId_WhenCalledByValidCustomerWithAtLeastOneRecord_Returns200WithListOfUserPaymentResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetAllPaymentByUserId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(new List<PaymentResponseDto>()
            {
                new PaymentResponseDto
                {
                    Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                    Type = "UPI",
                    Name = "GPay",
                    PaymentValue = "mygpayupi@paytm"                
                }
            });
        }

        [Fact]
        public void GetAllPaymentByUserId_WhenCalledByValidCustomerWithNoRecords_Returns204()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            IActionResult result = controller.GetAllPaymentByUserId(Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"));

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public void GetAllPaymentByUserId_WhenCalledByCustomerToAccessOtherAccountPaymentDetail_ThrowsNotFound()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetAllPaymentByUserId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetAllPaymentByUserId_WhenCalledByAdminToAccessOtherAccountPaymentDetail_Returns200WithListOfUserPaymentResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetAllPaymentByUserId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(new List<PaymentResponseDto>()
            {
                new PaymentResponseDto
                {
                    Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                    Type = "UPI",
                    Name = "GPay",
                    PaymentValue = "mygpayupi@paytm"
                }
            });
        }

        [Fact]
        public void GetPaymentByPaymentId_WhenCalledByValidUserId_Returns200WithPaymentResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new PaymentResponseDto
            {
                Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                Type = "UPI",
                Name = "GPay",
                PaymentValue = "mygpayupi@paytm"
            });
        }

        [Fact]
        public void GetPaymentByPaymentId_WhenCalledByUserToAccessOtherUserPayment_ThrowNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetPaymentByPaymentId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void GetPaymentByPaymentId_WhenCalledByAdminToAccessOtherPaymentDetail_Returns200WithPaymentResponseDto()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult result = controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            ObjectResult okResult = result.Should().BeOfType<ObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().BeEquivalentTo(new PaymentResponseDto
            {
                Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                Type = "UPI",
                Name = "GPay",
                PaymentValue = "mygpayupi@paytm"
            });
        }
        
        [Fact]
        public void GetPaymentByPaymentId_WhenCalledByUserWhoDoesNotHaveRequestedPayment_ThrowNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"));

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found");
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenCalledByValidUserToUpdateTheirPaymentDetailWithValidValues_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            };

            IActionResult updatedResult = controller.UpdatePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"), updatedPayment);

            IActionResult resultGetUpdatedPayment = controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            StatusCodeResult okResultUpdate = updatedResult.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUpdatedPayment.Should().BeOfType<ObjectResult>().Subject;

            okResultGet.Value.Should().BeEquivalentTo(new PaymentResponseDto
            {
                Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                Type = "UPI",
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            });
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenCalledByACustomerToUpdateOtherAccountPaymentWithValidValues_ThrowNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            };

            Action act = () => controller.UpdatePaymentByPaymentId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"), updatedPayment);

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenCalledByAnAdminToUpdateOtherAccountPaymentWithValidValues_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            };

            IActionResult updatedResult = controller.UpdatePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"), updatedPayment);

            IActionResult resultGetUpdatedPayment = controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            StatusCodeResult okResultUpdate = updatedResult.Should().BeOfType<StatusCodeResult>().Subject;
            okResultUpdate.StatusCode.Should().Be(StatusCodes.Status200OK);

            ObjectResult okResultGet = resultGetUpdatedPayment.Should().BeOfType<ObjectResult>().Subject;

            okResultGet.Value.Should().BeEquivalentTo(new PaymentResponseDto
            {
                Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                Type = "UPI",
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            });
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenPaymentIdDoesNotExistsForTheUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "PAYTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            };

            Action act = () => controller.UpdatePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"), updatedPayment);

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found");
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenPaymentNameAlreadyExistsForTheUser_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "GPay",
                PaymentValue = "mygpayupi@paytm"
            };

            Action act = () => controller.UpdatePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"), updatedPayment);

            act.Should().Throw<ConflictException>().WithMessage("Name already exists");
        }

        [Fact]
        public void UpdatePaymentByPaymentId_WhenPaymentValueAlreadyExistsForTheUser_ThrowsConflictException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            PaymentUpdateDto updatedPayment = new PaymentUpdateDto
            {
                Name = "Personal",
                PaymentValue = "mygpayupi@paytm"
            };

            Action act = () => controller.UpdatePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"), updatedPayment);

            act.Should().Throw<ConflictException>().WithMessage("Payment value already exists");
        }

        [Fact]
        public void DeletePaymentByPaymentId_WhenCalledByUserToDeleteOwnPaymentDetail_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult resultDelete = controller.DeletePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            Action act = () => controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found");
        }

        [Fact]
        public void DeletePaymentByPaymentId_WhenCalledByUserToDeleteOtherUserPayment_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeletePaymentByPaymentId(Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"), Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"));

            act.Should().Throw<NotFoundException>().WithMessage("No user account has been found");
        }

        [Fact]
        public void DeletePaymentByPaymentId_WhenCalledByAdminToDeleteOtherUserPayment_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"));

            IActionResult resultDelete = controller.DeletePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            Action act = () => controller.GetPaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"));

            StatusCodeResult okResult = resultDelete.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found");
        }

        [Fact]
        public void DeletePaymentByPaymentId_WhenCalledByUserToDeletePaymentThatDoesNotExists_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.DeletePaymentByPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("123f4c63-f510-4ae1-95c1-4e5a43c42c07"));

            act.Should().Throw<NotFoundException>().WithMessage("No payment record has been found");
        }

        [Fact]
        public void VerifyPaymentId_WhenPaymentIdExistsForTheUser_Returns200()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            IActionResult result = controller.VerifyPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"));

            StatusCodeResult okResult = result.Should().BeOfType<StatusCodeResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public void VerifyPaymentId_WhenPaymentIdDoesNotExistsForTheUser_ThrowsNotFoundException()
        {
            SetUp(TestHelper.GetContextWithRecords(), Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"));

            Action act = () => controller.VerifyPaymentId(Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"), Guid.Parse("12332104-c943-4716-aef0-feeea5ac8cce"));

            act.Should().Throw<NotFoundException>().WithMessage("No payment information has been found for the user");
        }
    }
}
