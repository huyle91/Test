using InfertilityTreatment.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;
        private readonly bool _enableSsl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpHost = _configuration["SmtpSettings:Host"] ?? throw new ArgumentNullException("SmtpSettings:Host not configured.");
            _smtpPort = _configuration.GetValue<int>("SmtpSettings:Port");
            _smtpUser = _configuration["SmtpSettings:Username"] ?? throw new ArgumentNullException("SmtpSettings:Username not configured.");
            _smtpPass = _configuration["SmtpSettings:Password"] ?? throw new ArgumentNullException("SmtpSettings:Password not configured.");
            _fromEmail = _configuration["SmtpSettings:FromEmail"] ?? throw new ArgumentNullException("SmtpSettings:FromEmail not configured.");
            _enableSsl = _configuration.GetValue<bool>("SmtpSettings:EnableSsl");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var fromAddress = new MailAddress(_fromEmail, "Trung tâm Hiếm muộn");
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            })
            {
                try
                {
                    await smtp.SendMailAsync(message);
                    _logger.LogInformation("Email sent to {ToEmail} successfully. Subject: {Subject}", toEmail, subject);
                }
                catch (SmtpException ex)
                {
                    _logger.LogError(ex, "SMTP Error sending email to {ToEmail}. Status: {StatusCode}. Message: {Message}",
                                    toEmail, ex.StatusCode, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General Error sending email to {ToEmail}. Message: {Message}",
                                     toEmail, ex.Message);
                    throw;
                }
            }
        }
    }
}
