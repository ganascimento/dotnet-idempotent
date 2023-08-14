namespace DotNet.Idempotent.Api.Infra.Cache.interfaces;

public interface ICacheRepository
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
}