using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyToDoList.Database;
using MyToDoList.Models;
using MyToDoList.Extensions;
using Microsoft.AspNetCore.Identity;
using MyToDoList.Services;
using Microsoft.Extensions.Configuration;

namespace MyToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToDosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private ApplicationUser _currentUser;
        private IMailService _mailService;
        private IConfiguration _configuration;

        public ToDosController(AppDbContext context, IMailService mailService, IConfiguration configuration)
        {
            _context = context;
            _mailService = mailService;
            _configuration = configuration;
        }

        // GET: api/ToDos
        [HttpGet]
        public async Task<IEnumerable<ToDo>> GetToDo()
        {
            _currentUser = HttpContext.GetCurrentUser();
            return await (from todo in _context.ToDos where todo.User.Id == _currentUser.Id select todo).ToListAsync();
        }

        // GET: api/ToDos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDo([FromRoute] int id)
        {
            _currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            if (toDo.User.Id != _currentUser.Id)
            {
                return Forbid();
            }
            
            return Ok(toDo);
        }

        // PUT: api/ToDos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo([FromRoute] int id, [FromBody] ToDo toDo)
        {
            _currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fromDb = await _context.ToDos.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (fromDb == null)
            {
                return BadRequest();
            }

            if (id != toDo.Id)
            {
                return BadRequest();
            }

            if (fromDb.User.Id != _currentUser.Id)
            {
                return Forbid();
            }

            _context.Entry(toDo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ToDos
        [HttpPost]
        public async Task<IActionResult> PostToDo([FromBody] ToDo toDo)
        {
            _currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            toDo.UserId = _currentUser.Id;
            _context.ToDos.Add(toDo);
            await _context.SaveChangesAsync();
            await _mailService.SendAsync(_configuration["DefaultFromEmail"], _currentUser.Email, "New To-Do Added", $"You have successfully added \"{toDo.Title}\" to MyToDoList");
            return CreatedAtAction("GetToDo", new { id = toDo.Id }, toDo);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateToDoState([FromRoute] int id)
        {
            _currentUser = HttpContext.GetCurrentUser();
            var todo = await _context.ToDos.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            if(todo.UserId != _currentUser.Id)
            {
                return Forbid();
            }
            todo.IsCompleted = !todo.IsCompleted;
            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        // DELETE: api/ToDos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo([FromRoute] int id)
        {
            _currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            if(toDo.UserId != _currentUser.Id)
            {
                return Forbid();
            }

            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();
            await _mailService.SendAsync(_configuration["DefaultFromEmail"], _currentUser.Email, "Deleted ToDo", $"Removed ToDo {toDo.Title} from MyToDoList");
            return Ok(toDo);
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.Id == id);
        }
    }
}