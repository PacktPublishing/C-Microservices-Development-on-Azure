using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToDosService.Data;
using ToDosService.Models;

namespace ToDosService.Controllers
{
    [Route("/api/todos")]
    [ApiController]
    [Authorize]
    public class ToDosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IConfiguration _configuration;

        public ToDosController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private int GetUserId()
        {
            int userId;
            Int32.TryParse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out userId);
            return userId;
        }

        // GET: api/ToDos
        [HttpGet]
        public async Task<IEnumerable<ToDo>> GetToDo()
        {
            int userId = GetUserId();
            return await (from todo in _context.ToDos where todo.UserId == userId select todo).ToListAsync();
        }

        // GET: api/ToDos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDo([FromRoute] int id)
        {
            //_currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            //if (toDo.User.Id != _currentUser.Id)
            //{
            //    return Forbid();
            //}

            return Ok(toDo);
        }

        // PUT: api/ToDos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo([FromRoute] int id, [FromBody] ToDo toDo)
        {
            //_currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fromDb = await _context.ToDos.FirstOrDefaultAsync(t => t.Id == id);
            if (fromDb == null)
            {
                return BadRequest();
            }

            if (id != toDo.Id)
            {
                return BadRequest();
            }

            //if (fromDb.User.Id != _currentUser.Id)
            //{
            //    return Forbid();
            //}

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
            int userId = GetUserId();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            toDo.UserId = userId;
            _context.ToDos.Add(toDo);
            await _context.SaveChangesAsync();
            ServicePartitionResolver servicePartitionResolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = await servicePartitionResolver.ResolveAsync(new Uri("fabric:/MyToDoListAttempt2/MailService"), new ServicePartitionKey(), CancellationToken.None);
            string endpoint = JObject.Parse(partition.GetEndpoint().Address)["Endpoints"].First.First.ToString();
            string email = HttpContext.User.FindFirst("Email").Value;
            using (var httpClient = new HttpClient())
            {
                dynamic emailDto = new
                {
                    To = email,
                    Subject = "New ToDo Added",
                    Content = toDo.Title
                };
                string emailDtoJson = JsonConvert.SerializeObject(emailDto);
                await httpClient.PostAsync(endpoint, new StringContent(emailDtoJson));
            }
            return CreatedAtAction("GetToDo", new { id = toDo.Id }, toDo);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateToDoState([FromRoute] int id)
        {
            //_currentUser = HttpContext.GetCurrentUser();
            var todo = await _context.ToDos.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }
            //if (todo.UserId != _currentUser.Id)
            //{
            //    return Forbid();
            //}
            todo.IsCompleted = !todo.IsCompleted;
            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        // DELETE: api/ToDos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo([FromRoute] int id)
        {
            //_currentUser = HttpContext.GetCurrentUser();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            //if (toDo.UserId != _currentUser.Id)
            //{
            //    return Forbid();
            //}

            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();
            //await _mailService.SendAsync(_configuration["DefaultFromEmail"], _currentUser.Email, "Deleted ToDo", $"Removed ToDo {toDo.Title} from MyToDoList");
            return Ok(toDo);
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.Id == id);
        }
    }
}