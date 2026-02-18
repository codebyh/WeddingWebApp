using Microsoft.AspNetCore.Mvc;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserClient client;

        public UsersController(IUserClient client) => this.client = client;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<User>>> GetAll() =>
            Ok(await client.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await client.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User input)
        {
            var created = await client.CreateAsync(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(string id, [FromBody] User input)
        {
            var updated = await client.UpdateAsync(id, input);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await client.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
