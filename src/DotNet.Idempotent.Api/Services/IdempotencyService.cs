using System.Text.Json;
using DotNet.Idempotent.Api.Infra.Cache.interfaces;
using DotNet.Idempotent.Api.Models.Dto;
using DotNet.Idempotent.Api.Services.Interfaces;

namespace DotNet.Idempotent.Api.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly ICacheRepository _cacheRepository;

    public IdempotencyService(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }

    public async Task<bool> RequestExistsAsync(Guid requestId)
    {
        var result = await _cacheRepository.GetAsync(requestId.ToString());
        if (result == null) return false;
        return true;
    }

    public async Task CreateRequestAsync(Guid requestId)
    {
        var idempotency = new IdempotencyDto
        {
            RequestId = requestId,
            CreatedOn = DateTime.UtcNow
        };

        await _cacheRepository.SetAsync(requestId.ToString(), JsonSerializer.Serialize(idempotency));
    }
}