namespace WeddingWebApp.Security;

public sealed class NullAdminCredentialStore : IAdminCredentialStore
{
    public Task<AdminCredential?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        Task.FromResult<AdminCredential?>(null);
}
