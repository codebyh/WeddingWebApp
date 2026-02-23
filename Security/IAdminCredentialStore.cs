namespace WeddingWebApp.Security;

public interface IAdminCredentialStore
{
    Task<AdminCredential?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
