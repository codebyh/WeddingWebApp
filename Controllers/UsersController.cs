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
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserClient client, ILogger<UsersController> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublicUser>> GetById(string id)
        {
            var user = await client.GetByIdAsync(id);
            if (user is null)
            {
                logger.LogInformation("Public user lookup returned 404 for id {UserId}.", id);
                return NotFound();
            }

            logger.LogInformation("Public user lookup succeeded for id {UserId}.", id);
            return Ok(new PublicUser
            {
                Name = user.DisplayName,
                Gender = user.Gender
            });
        }
    }

}
