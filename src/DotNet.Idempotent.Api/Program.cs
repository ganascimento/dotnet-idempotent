using DotNet.Idempotent.Api.Infra.Cache;
using DotNet.Idempotent.Api.Infra.Cache.interfaces;
using DotNet.Idempotent.Api.Services;
using DotNet.Idempotent.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IIdempotencyService, IdempotencyService>();
builder.Services.AddSingleton<ICacheRepository, CacheRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
