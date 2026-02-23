using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace WeddingWebApp.Security;

public sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Basic";
    private readonly IAdminCredentialStore adminCredentialStore;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IAdminCredentialStore adminCredentialStore)
        : base(options, logger, encoder)
    {
        this.adminCredentialStore = adminCredentialStore;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeader))
            return AuthenticateResult.NoResult();

        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var parsedValue))
            return AuthenticateResult.Fail("Invalid Authorization header.");

        if (!SchemeName.Equals(parsedValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        if (string.IsNullOrWhiteSpace(parsedValue.Parameter))
            return AuthenticateResult.Fail("Missing Basic credentials.");

        string rawCredentials;
        try
        {
            var credentialBytes = Convert.FromBase64String(parsedValue.Parameter);
            rawCredentials = Encoding.UTF8.GetString(credentialBytes);
        }
        catch (FormatException)
        {
            return AuthenticateResult.Fail("Invalid Basic credentials encoding.");
        }

        var separatorIndex = rawCredentials.IndexOf(':');
        if (separatorIndex <= 0)
            return AuthenticateResult.Fail("Invalid Basic credential format.");

        var username = rawCredentials[..separatorIndex];
        var password = rawCredentials[(separatorIndex + 1)..];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return AuthenticateResult.Fail("Username and password are required.");

        var credential = await adminCredentialStore.GetByUsernameAsync(username, Context.RequestAborted);
        if (credential is null || !credential.IsActive || string.IsNullOrWhiteSpace(credential.Username))
            return AuthenticateResult.Fail("Invalid username or password.");

        var passwordValid = AdminPasswordHasher.VerifyPassword(
            password,
            credential.PasswordHash,
            credential.PasswordSalt,
            credential.PasswordIterations,
            credential.PasswordHashAlgorithm);
        if (!passwordValid)
            return AuthenticateResult.Fail("Invalid username or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, credential.Username),
            new Claim(ClaimTypes.Name, credential.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers[HeaderNames.WWWAuthenticate] = "Basic realm=\"WeddingWebApp API\", charset=\"UTF-8\"";
        return base.HandleChallengeAsync(properties);
    }
}
