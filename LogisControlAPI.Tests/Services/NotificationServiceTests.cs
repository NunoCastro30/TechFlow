using Xunit;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Services;
using LogisControlAPI.Models;
using System.Threading.Tasks;
using System;

/// <summary>
/// Fake do NotificationService para simular envio de emails nos testes sem falhas.
/// </summary>
public class FakeNotificationService : NotificationService
{
    public FakeNotificationService() : base(null) { }

    public override async Task NotificarAsync(string destinatario, string assunto, string mensagem)
    {
        await Task.CompletedTask;
    }
}

    
