using System.ComponentModel.DataAnnotations.Schema;
using Bogus;

namespace _03_AzureFunction_QueueTrigger.Domain.Entities;

public record User
{
    public int Id { get; init; }

    [Column(TypeName = "nvarchar(255)")]
    public string ExternalID { get; init; }

    [Column(TypeName = "nvarchar(255)")]
    public string FirstName { get; init; } = null!;

    [Column(TypeName = "nvarchar(255)")]
    public string LastName { get; init; } = null!;

    [Column(TypeName = "nvarchar(100)")]
    public string Email { get; init; } = null!;

    public static User CreateFake(string externalID)
    {
        return new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.ExternalID, externalID);
    }


    public string RowKey => $"User_{ExternalID}";
}