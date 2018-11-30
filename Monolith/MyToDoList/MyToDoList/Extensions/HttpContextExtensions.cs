using Microsoft.AspNetCore.Http;
using MyToDoList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDoList.Extensions
{
    public static class HttpContextExtensions
    {
        public static ApplicationUser GetCurrentUser(this HttpContext context)
        {
            try
            {
                object currentUser;
                if (!context.Items.TryGetValue("CurrentUser", out currentUser))
                {
                    return null;
                }
                else
                {
                    return currentUser as ApplicationUser;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
