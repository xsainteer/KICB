using Domain.Entities;

namespace Tests.Entities;

public class ClientTests
{
    private static readonly DateOnly DateOfBirth = new(1990, 5, 17);

    [Fact]
    public void Constructor_SetsDetails()
    {
        var client = new Client("Ivanov", "Ivan", "Ivanovich", "ivan@example.com", DateOfBirth);

        Assert.NotEqual(Guid.Empty, client.Id);
        Assert.Equal("Ivanov", client.LastName);
        Assert.Equal("Ivan", client.FirstName);
        Assert.Equal("Ivanovich", client.MiddleName);
        Assert.Equal("ivan@example.com", client.Email);
        Assert.Equal(DateOfBirth, client.DateOfBirth);
        Assert.Empty(client.Phones);
        Assert.Empty(client.Accounts);
    }

    [Fact]
    public void Constructor_WithoutMiddleName_Succeeds()
    {
        var client = new Client("Ivanov", "Ivan", null, "ivan@example.com", DateOfBirth);

        Assert.Null(client.MiddleName);
    }

    [Theory]
    [InlineData("", "Ivan", "ivan@example.com")]
    [InlineData("Ivanov", " ", "ivan@example.com")]
    [InlineData("Ivanov", "Ivan", "")]
    public void Constructor_WithBlankRequiredField_Throws(string lastName, string firstName, string email)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Client(lastName, firstName, null, email, DateOfBirth));
    }

    [Fact]
    public void Update_ChangesDetails()
    {
        var client = new Client("Ivanov", "Ivan", null, "ivan@example.com", DateOfBirth);
        var newDateOfBirth = new DateOnly(1991, 1, 1);

        client.Update("Petrov", "Petr", "Petrovich", "petr@example.com", newDateOfBirth);

        Assert.Equal("Petrov", client.LastName);
        Assert.Equal("Petr", client.FirstName);
        Assert.Equal("Petrovich", client.MiddleName);
        Assert.Equal("petr@example.com", client.Email);
        Assert.Equal(newDateOfBirth, client.DateOfBirth);
    }

    [Fact]
    public void Update_WithBlankRequiredField_ThrowsAndKeepsDetails()
    {
        var client = new Client("Ivanov", "Ivan", null, "ivan@example.com", DateOfBirth);

        Assert.ThrowsAny<ArgumentException>(
            () => client.Update("Petrov", "Petr", null, " ", DateOfBirth));

        Assert.Equal("Ivanov", client.LastName);
        Assert.Equal("ivan@example.com", client.Email);
    }
}
