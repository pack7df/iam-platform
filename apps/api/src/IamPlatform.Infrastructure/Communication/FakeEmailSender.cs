using IamPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace IamPlatform.Infrastructure.Communication;

public sealed class FakeEmailSender : IEmailSender
{
    private readonly ILogger<FakeEmailSender> _logger;

    public FakeEmailSender(ILogger<FakeEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending email to {To} with subject {Subject} and body {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
