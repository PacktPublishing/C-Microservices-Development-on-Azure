using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailService.Models;
using MailService.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Controllers
{
    [Route("api/mails")]
    [ApiController]
    public class MailsController : Controller
    {
        private IMailService _mailService;
        private string _fromEmail = Environment.GetEnvironmentVariable("DEFAULT_FROM_EMAIL");

        public MailsController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost]
        public IActionResult Send([FromBody] Email email)
        {
            _mailService.SendAsync(_fromEmail, email.To, email.Subject, email.Content);
            return Ok();
        }
    }
}