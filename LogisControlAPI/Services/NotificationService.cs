using LogisControlAPI.Interfaces;

namespace LogisControlAPI.Services
{
    /// <summary>
    /// Serviço responsável por enviar notificações por email.
    /// Utiliza a abstração IEmailSender, permitindo flexibilidade na implementação.
    /// </summary>
    public class NotificationService
    {
        private readonly IEmailSender _emailSender;

        public NotificationService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        /// <summary>
        /// Envia uma notificação por email.
        /// </summary>
        /// <param name="destinatario">Endereço de email do destinatário.</param>
        /// <param name="assunto">Assunto do email.</param>
        /// <param name="mensagem">Corpo da mensagem.</param>
        public async Task NotificarAsync(string destinatario, string assunto, string mensagem)
        {
            await _emailSender.EnviarAsync(destinatario, assunto, mensagem);
        }
    }
}
