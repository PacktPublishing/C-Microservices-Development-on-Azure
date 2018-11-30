using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDoList.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(string from, string to, string subject, string content);
    }
}
