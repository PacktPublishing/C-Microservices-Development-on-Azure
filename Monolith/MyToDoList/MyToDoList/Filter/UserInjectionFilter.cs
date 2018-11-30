using Microsoft.AspNetCore.Mvc.Filters;
using MyToDoList.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyToDoList.Models;

namespace MyToDoList.Filter
{
    public class UserInjectionFilter : IAsyncActionFilter
    {
        UserManager<ApplicationUser> _userManager;

        public UserInjectionFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var currentUser = await _userManager.GetUserAsync(context.HttpContext.User);
            if (currentUser != null)
            {
                context.HttpContext.Items.Add("CurrentUser", currentUser);
            }
            await next();
        }
    }
}
