using Domain.Entities;
using Domain.Enums;

namespace Tests.Entities;

public class PhoneTests
{
    [Fact]
    public void Constructor_SetsDetails()
    {
        var clientId = Guid.NewGuid();

        var phone = new Phone(clientId, "+996555123456", PhoneType.Mobile);

        Assert.NotEqual(Guid.Empty, phone.Id);
        Assert.Equal(clientId, phone.ClientId);
        Assert.Equal("+996555123456", phone.Number);
        Assert.Equal(PhoneType.Mobile, phone.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithBlankNumber_Throws(string? number)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Phone(Guid.NewGuid(), number!, PhoneType.Mobile));
    }

    [Fact]
    public void Update_ChangesDetails()
    {
        var phone = new Phone(Guid.NewGuid(), "+996555123456", PhoneType.Mobile);

        phone.Update("+996312654321", PhoneType.Work);

        Assert.Equal("+996312654321", phone.Number);
        Assert.Equal(PhoneType.Work, phone.Type);
    }

    [Fact]
    public void Update_WithBlankNumber_ThrowsAndKeepsDetails()
    {
        var phone = new Phone(Guid.NewGuid(), "+996555123456", PhoneType.Mobile);

        Assert.ThrowsAny<ArgumentException>(() => phone.Update("", PhoneType.Work));

        Assert.Equal("+996555123456", phone.Number);
        Assert.Equal(PhoneType.Mobile, phone.Type);
    }
}
