using Homework.Application.Commands.Users;
using Homework.Application.Services;
using Moq;

namespace Homework.UnitTests.Commands.Users;

[TestFixture]
public class ConsumeUserSessionCommandHandlerTests
{
    [Test]
    public async Task Handle_ValidSessionId_ReturnsCorrectResult()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var expectedEmail = "test@example.com";

        var mockAdminService = new Mock<IAdministrationService>();
        mockAdminService.Setup(service => service.ConsumeUserSession(sessionId))
                        .ReturnsAsync(expectedEmail);

        var handler = new ConsumeUserSessionCommandHandler(mockAdminService.Object);

        var request = new ConsumeUserSessionCommand { SessionId = sessionId };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(expectedEmail, result.Data.Email);

        mockAdminService.Verify(service => service.ConsumeUserSession(sessionId), Times.Once);
    }
}
