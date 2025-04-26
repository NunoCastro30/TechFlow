using Xunit;
using Moq;
using LogisControlAPI.Interfaces;
using LogisControlAPI.Services;
using System.Threading.Tasks;

namespace LogisControlAPI.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task EnviarEmailAsync_DeveChamarEmailSender()
        {
            // Arrange
            var mockEmailSender = new Mock<IEmailSender>();
            var service = new NotificationService(mockEmailSender.Object);

            string destinatario = "teste@email.com";
            string assunto = "Assunto Teste";
            string mensagem = "Mensagem de teste";

            // Act
            await service.NotificarAsync(destinatario, assunto, mensagem);

            // Assert
            mockEmailSender.Verify(
                s => s.EnviarAsync(destinatario, assunto, mensagem),
                Times.Once
            );
        }

        [Fact]
        public async Task EnviarEmailAsync_NaoLancaExcecao()
        {
            // Arrange
            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender
                .Setup(s => s.EnviarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new NotificationService(mockEmailSender.Object);

            // Act & Assert
            var ex = await Record.ExceptionAsync(() =>
                service.NotificarAsync("teste@email.com", "Assunto", "Corpo")
            );

            Assert.Null(ex); 
        }
    }
}
