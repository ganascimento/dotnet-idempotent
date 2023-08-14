using DotNet.Idempotent.Api.Infra.Cache.interfaces;
using StackExchange.Redis;

namespace DotNet.Idempotent.Api.Infra.Cache;

public class CacheRepository : ICacheRepository
{
    private readonly IConfiguration _configuration;

    public CacheRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string?> GetAsync(string key)
    {
        var db = await this.GetDbAsync();
        string? result = await db.StringGetAsync(key);

        return result;
    }

    public async Task SetAsync(string key, string value)
    {
        var db = await this.GetDbAsync();
        await db.StringSetAsync(key, value);
    }

    private async Task<IDatabase> GetDbAsync()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync(_configuration["ConnectionStrings:Redis"]);
        var db = redis.GetDatabase(0);
        return db;
    }
}