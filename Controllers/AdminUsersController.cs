using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeddingWebApp.Services;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Models;
using WeddingWebApp.Security;
using WeddingWebApp.Validation;

namespace WeddingWebApp.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationHandler.SchemeName)]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserClient client;
        private readonly ILogger<AdminUsersController> logger;

        public AdminUsersController(IUserClient client, ILogger<AdminUsersController> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<User>>> GetAll()
        {
            logger.LogInformation("Admin requested all users.");
            return Ok(await client.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await client.GetByIdAsync(id);
            if (user is null)
            {
                logger.LogWarning("Admin user lookup failed for id {UserId}.", id);
                return NotFound();
            }

            logger.LogInformation("Admin fetched user {UserId}.", id);
            return Ok(user);
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetByIdPdf(string id)
        {
            var user = await client.GetByIdAsync(id);
            if (user is null)
            {
                logger.LogWarning("Admin PDF lookup failed for id {UserId}.", id);
                return NotFound();
            }

            var pdfBytes = AdminUserPdfGenerator.Generate(user);
            var baseName = string.IsNullOrWhiteSpace(user.DisplayName) ? "profile" : user.DisplayName.Trim().Replace(' ', '-');
            var fileName = $"{baseName}-{id}.pdf";
            logger.LogInformation("Admin generated PDF for user {UserId}.", id);
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User input)
        {
            if (!AdminUserCreateValidation.TryBuildCreateUser(input, out var createUser, out var error))
            {
                logger.LogWarning("Admin create user validation failed: {ValidationError}", error);
                return BadRequest(error);
            }

            var created = await client.CreateAsync(createUser);
            logger.LogInformation("Admin created user {UserId}.", created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(string id, [FromBody] User input)
        {
            var updated = await client.UpdateAsync(id, input);
            if (updated is null)
            {
                logger.LogWarning("Admin update failed for id {UserId}. User not found.", id);
                return NotFound();
            }

            logger.LogInformation("Admin updated user {UserId}.", id);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await client.DeleteAsync(id);
            if (!ok)
            {
                logger.LogWarning("Admin delete failed for id {UserId}. User not found.", id);
                return NotFound();
            }

            logger.LogInformation("Admin deleted user {UserId}.", id);
            return NoContent();
        }
    }
}
