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

        [HttpGet("{id}")]
        public async Task<ActionResult<PublicUser>> GetById(string id)
        {
            var user = await client.GetByIdAsync(id);
            return user is null
                ? NotFound()
                : Ok(new PublicUser
                {
                    Name = user.DisplayName,
                    Gender = user.Gender
                });
        }
    }

}
