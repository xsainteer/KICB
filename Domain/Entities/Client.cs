namespace Domain.Entities;

public class Client
{
    private readonly List<Phone> _phones = new();
    private readonly List<Account> _accounts = new();

    private Client()
    {
    } // EF Core

    public Client(string lastName, string firstName, string? middleName, string email, DateOnly dateOfBirth)
    {
        Id = Guid.NewGuid();
        Update(lastName, firstName, middleName, email, dateOfBirth);
    }

    public Guid Id { get; private set; }

    public string LastName { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string? MiddleName { get; private set; }

    public string Email { get; private set; } = null!;
    public DateOnly DateOfBirth { get; private set; }

    public IReadOnlyCollection<Phone> Phones => _phones.AsReadOnly();
    public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

    public void Update(string lastName, string firstName, string? middleName, string email, DateOnly dateOfBirth)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Email = email;
        DateOfBirth = dateOfBirth;
    }
}
