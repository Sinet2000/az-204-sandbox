namespace _03_AzureFunction_QueueTrigger.Data.Entities;

public record User
{
    public int Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
}