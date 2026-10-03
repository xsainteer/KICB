using Domain.Entities;
using Domain.Enums;

namespace Tests.Entities;

public class AccountTests
{
    private static readonly DateOnly CardExpiration = new(2030, 1, 31);

    private static Account NewAccount(Currency currency = Currency.KGS, decimal balance = 0)
    {
        var account = new Account(Guid.NewGuid(), "KG0001", currency);
        if (balance > 0)
            account.Deposit(balance);
        return account;
    }

    private static Account NewClosedAccount()
    {
        var account = NewAccount();
        account.Close();
        return account;
    }

    [Fact]
    public void Constructor_CreatesActiveAccountWithZeroBalance()
    {
        var clientId = Guid.NewGuid();

        var account = new Account(clientId, "KG0001", Currency.USD);

        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(clientId, account.ClientId);
        Assert.Equal("KG0001", account.Number);
        Assert.Equal(Currency.USD, account.Currency);
        Assert.Equal(0, account.Balance);
        Assert.Equal(AccountStatus.Active, account.Status);
        Assert.Empty(account.Cards);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithBlankNumber_Throws(string? number)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Account(Guid.NewGuid(), number!, Currency.KGS));
    }

    [Fact]
    public void Deposit_IncreasesBalance()
    {
        var account = NewAccount(balance: 100);

        var result = account.Deposit(50);

        ResultAssert.Success(result);
        Assert.Equal(150, account.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deposit_WithNonPositiveAmount_Throws(decimal amount)
    {
        var account = NewAccount();

        Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(amount));
    }

    [Fact]
    public void Deposit_ToClosedAccount_ReturnsInactive()
    {
        var account = NewClosedAccount();

        var result = account.Deposit(50);

        ResultAssert.Failure(result, "Account.Inactive");
        Assert.Equal(0, account.Balance);
    }

    [Fact]
    public void Withdraw_DecreasesBalance()
    {
        var account = NewAccount(balance: 100);

        var result = account.Withdraw(30);

        ResultAssert.Success(result);
        Assert.Equal(70, account.Balance);
    }

    [Fact]
    public void Withdraw_EntireBalance_Succeeds()
    {
        var account = NewAccount(balance: 100);

        var result = account.Withdraw(100);

        ResultAssert.Success(result);
        Assert.Equal(0, account.Balance);
    }

    [Fact]
    public void Withdraw_MoreThanBalance_ReturnsInsufficientFundsAndKeepsBalance()
    {
        var account = NewAccount(balance: 100);

        var result = account.Withdraw(100.01m);

        ResultAssert.Failure(result, "Account.InsufficientFunds");
        Assert.Equal(100, account.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Withdraw_WithNonPositiveAmount_Throws(decimal amount)
    {
        var account = NewAccount(balance: 100);

        Assert.Throws<ArgumentOutOfRangeException>(() => account.Withdraw(amount));
    }

    [Fact]
    public void Withdraw_FromClosedAccount_ReturnsInactive()
    {
        var account = NewClosedAccount();

        var result = account.Withdraw(1);

        ResultAssert.Failure(result, "Account.Inactive");
    }

    [Fact]
    public void TransferTo_MovesFundsBetweenAccounts()
    {
        var source = NewAccount(balance: 100);
        var destination = NewAccount(balance: 10);

        var result = source.TransferTo(destination, 40);

        ResultAssert.Success(result);
        Assert.Equal(60, source.Balance);
        Assert.Equal(50, destination.Balance);
    }

    [Fact]
    public void TransferTo_EntireBalance_Succeeds()
    {
        var source = NewAccount(balance: 100);
        var destination = NewAccount();

        var result = source.TransferTo(destination, 100);

        ResultAssert.Success(result);
        Assert.Equal(0, source.Balance);
        Assert.Equal(100, destination.Balance);
    }

    [Fact]
    public void TransferTo_SameAccount_ReturnsSameAccount()
    {
        var account = NewAccount(balance: 100);

        var result = account.TransferTo(account, 40);

        ResultAssert.Failure(result, "Account.SameAccount");
        Assert.Equal(100, account.Balance);
    }

    [Fact]
    public void TransferTo_FromClosedAccount_ReturnsInactiveAndKeepsBalances()
    {
        var source = NewClosedAccount();
        var destination = NewAccount(balance: 10);

        var result = source.TransferTo(destination, 1);

        ResultAssert.Failure(result, "Account.Inactive");
        Assert.Contains(source.Id.ToString(), result.Error!.Message);
        Assert.Equal(10, destination.Balance);
    }

    [Fact]
    public void TransferTo_ClosedDestination_ReturnsInactiveAndKeepsBalances()
    {
        var source = NewAccount(balance: 100);
        var destination = NewClosedAccount();

        var result = source.TransferTo(destination, 40);

        ResultAssert.Failure(result, "Account.Inactive");
        Assert.Contains(destination.Id.ToString(), result.Error!.Message);
        Assert.Equal(100, source.Balance);
        Assert.Equal(0, destination.Balance);
    }

    [Fact]
    public void TransferTo_DifferentCurrency_ReturnsCurrencyMismatchAndKeepsBalances()
    {
        var source = NewAccount(Currency.KGS, balance: 100);
        var destination = NewAccount(Currency.USD, balance: 10);

        var result = source.TransferTo(destination, 40);

        ResultAssert.Failure(result, "Account.CurrencyMismatch");
        Assert.Equal(100, source.Balance);
        Assert.Equal(10, destination.Balance);
    }

    [Fact]
    public void TransferTo_MoreThanBalance_ReturnsInsufficientFundsAndKeepsBalances()
    {
        var source = NewAccount(balance: 100);
        var destination = NewAccount(balance: 10);

        var result = source.TransferTo(destination, 150);

        ResultAssert.Failure(result, "Account.InsufficientFunds");
        Assert.Equal(100, source.Balance);
        Assert.Equal(10, destination.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TransferTo_WithNonPositiveAmount_Throws(decimal amount)
    {
        var source = NewAccount(balance: 100);
        var destination = NewAccount();

        Assert.Throws<ArgumentOutOfRangeException>(() => source.TransferTo(destination, amount));
    }

    [Fact]
    public void IssueCard_AddsActiveCardLinkedToAccount()
    {
        var account = NewAccount();

        var result = account.IssueCard("4000000000000001", CardExpiration, CardType.Debit);

        ResultAssert.Success(result);
        var card = result.Value;
        Assert.Equal(CardStatus.Active, card.Status);
        Assert.Equal(account.Id, card.AccountId);
        Assert.Equal("4000000000000001", card.Number);
        Assert.Equal(CardExpiration, card.ExpirationDate);
        Assert.Equal(CardType.Debit, card.Type);
        Assert.Contains(card, account.Cards);
    }

    [Fact]
    public void IssueCard_OnClosedAccount_ReturnsInactiveAndAddsNoCard()
    {
        var account = NewClosedAccount();

        var result = account.IssueCard("4000000000000001", CardExpiration, CardType.Debit);

        ResultAssert.Failure(result, "Account.Inactive");
        Assert.Empty(account.Cards);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IssueCard_WithBlankNumber_Throws(string? number)
    {
        var account = NewAccount();

        Assert.ThrowsAny<ArgumentException>(() => account.IssueCard(number!, CardExpiration, CardType.Debit));
    }

    [Fact]
    public void Close_WithZeroBalanceAndNoCards_ClosesAccount()
    {
        var account = NewAccount();

        var result = account.Close();

        ResultAssert.Success(result);
        Assert.Equal(AccountStatus.Closed, account.Status);
        Assert.True(account.IsClosed);
    }

    [Fact]
    public void Close_WithNonZeroBalance_ReturnsNotEmpty()
    {
        var account = NewAccount(balance: 0.01m);

        var result = account.Close();

        ResultAssert.Failure(result, "Account.NotEmpty");
        Assert.False(account.IsClosed);
    }

    [Fact]
    public void Close_WithActiveCard_ReturnsHasActiveCards()
    {
        var account = NewAccount();
        account.IssueCard("4000000000000001", CardExpiration, CardType.Debit);

        var result = account.Close();

        ResultAssert.Failure(result, "Account.HasActiveCards");
        Assert.False(account.IsClosed);
    }

    [Fact]
    public void Close_WithOnlyBlockedCards_ClosesAccount()
    {
        var account = NewAccount();
        var card = account.IssueCard("4000000000000001", CardExpiration, CardType.Credit).Value;
        account.BlockCard(card.Id);

        var result = account.Close();

        ResultAssert.Success(result);
        Assert.True(account.IsClosed);
    }

    [Fact]
    public void Close_AlreadyClosedAccount_ReturnsInactive()
    {
        var account = NewClosedAccount();

        var result = account.Close();

        ResultAssert.Failure(result, "Account.Inactive");
    }
}
