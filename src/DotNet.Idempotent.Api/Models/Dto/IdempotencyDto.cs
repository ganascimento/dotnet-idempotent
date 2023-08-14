namespace DotNet.Idempotent.Api.Models.Dto;

public class IdempotencyDto
{
    public Guid RequestId { get; set; }
    public DateTime CreatedOn { get; set; }
}