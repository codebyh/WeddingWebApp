
using System.Collections.Concurrent;
using WeddingWebApp.Data.Abstractions;
using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Data.InMemory;

public sealed class InMemoryUserClient : IUserClient
{
    private readonly ConcurrentDictionary<string, User> users = new();

    public Task<IReadOnlyCollection<User>> GetAllAsync() =>
        Task.FromResult((IReadOnlyCollection<User>)users.Values.ToList());

    public Task<User?> GetByIdAsync(string id)
    {
        users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User> CreateAsync(User user)
    {
        var id = string.IsNullOrWhiteSpace(user.Id)
            ? Guid.NewGuid().ToString("n")
            : user.Id;

        // Create a new instance so we control Id/CreatedUtc if desired
        var created = new User
        {
            Id = id,
            DisplayName = user.DisplayName,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            Email = user.Email,
            CreatedUtc = user.CreatedUtc == default ? DateTimeOffset.UtcNow : user.CreatedUtc
        };

        users[id] = created;
        return Task.FromResult(created);
    }

    public Task<User?> UpdateAsync(string id, User update)
    {
        // "Update logic": patch the existing user, keep CreatedUtc stable, force Id match
        if (!users.TryGetValue(id, out var existing))
            return Task.FromResult<User?>(null);

        var updated = new User
        {
            Id = id,
            CreatedUtc = existing.CreatedUtc,

            // Choose whether you want PATCH-like behavior or PUT-like behavior.
            // Here: PATCH-like (keep old value if update didn't send it)
            DisplayName = string.IsNullOrWhiteSpace(update.DisplayName) ? existing.DisplayName : update.DisplayName,
            BirthDate = update.BirthDate ?? existing.BirthDate,
            Gender = update.Gender ?? existing.Gender,
            Email = update.Email ?? existing.Email
        };

        users[id] = updated;
        return Task.FromResult<User?>(updated);
    }

    public Task<bool> DeleteAsync(string id) =>
        Task.FromResult(users.TryRemove(id, out _));
}

