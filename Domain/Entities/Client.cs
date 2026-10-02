namespace Domain.Entities;

public class Client
{
    public Guid Id { get; set; }

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }

    public string Email { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }

    public ICollection<Phone> Phones { get; set; } = new List<Phone>();
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
