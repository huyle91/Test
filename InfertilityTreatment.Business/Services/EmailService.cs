using InfertilityTreatment.Business.Interfaces;
using Microsoft.Extensions.Configuration;
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
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;
        private readonly bool _enableSsl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                    Console.WriteLine($"Email sent to {toEmail} successfully.");
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    throw new ApplicationException($"Error sending email to {toEmail}: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Email Error: {ex.Message}");
                    throw new ApplicationException($"Error sending email to {toEmail}: {ex.Message}", ex);
                }
            }
        }
    }
}
