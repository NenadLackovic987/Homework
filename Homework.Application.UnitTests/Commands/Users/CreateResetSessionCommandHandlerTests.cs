using Homework.Application.Commands.Users;
using Homework.Application.Services;
using Moq;

namespace Homework.UnitTests.Commands.Users
{
    [TestFixture]
    public class CreateResetSessionCommandHandlerTests
    {
        [Test]
        public async Task Handle_ValidEmail_ReturnsCorrectResult()
        {
            // Arrange
            var email = "test@example.com";
            var resetPassword = "generatedPassword";

            var mockAdminService = new Mock<IAdministrationService>();
            var mockEmailService = new Mock<IEmailService>();

            mockAdminService.Setup(service => service.CreateResetSession(It.IsAny<Guid>(), email))
                            .ReturnsAsync(resetPassword);

            var handler = new CreateResetSessionCommandHandler(mockAdminService.Object, mockEmailService.Object);

            var request = new CreateResetSessionCommand { Email = email };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);

            mockAdminService.Verify(service => service.CreateResetSession(It.IsAny<Guid>(), email), Times.Once);

            mockEmailService.Verify(service => service.SendResetPasswordEmail(email, It.IsAny<Guid>(), resetPassword), Times.Once);
        }
    }
}
