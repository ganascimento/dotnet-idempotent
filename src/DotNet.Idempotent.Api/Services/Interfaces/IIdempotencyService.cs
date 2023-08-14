namespace DotNet.Idempotent.Api.Services.Interfaces;

public interface IIdempotencyService
{
    Task<bool> RequestExistsAsync(Guid requestId);
    Task CreateRequestAsync(Guid requestId);
}