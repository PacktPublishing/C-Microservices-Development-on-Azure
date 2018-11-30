using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MailService.Services
{
    public class SendGridMailService : IMailService
    {
        private SendGridClient _sendGridClient;

        public SendGridMailService(SendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient;
        }

        public async Task<bool> SendAsync(string from, string to, string subject, string content)
        {
            var fromAddress = new EmailAddress(from);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, content, "");
            Response result = await _sendGridClient.SendEmailAsync(msg);
            return result.StatusCode == HttpStatusCode.Accepted || result.StatusCode == HttpStatusCode.OK;
        }
    }
}
