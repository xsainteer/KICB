using Domain.Entities;
using Domain.Enums;

namespace Tests.Entities;

/// <summary>
/// Card lifecycle, driven through the <see cref="Account"/> aggregate root.
/// </summary>
public class CardTests
{
    private static (Account Account, Card Card) NewCard()
    {
        var account = new Account(Guid.NewGuid(), "KG0001", Currency.KGS);
        var card = account.IssueCard("4000000000000001", new DateOnly(2030, 1, 31), CardType.Debit).Value;
        return (account, card);
    }

    [Fact]
    public void BlockCard_ActiveCard_BlocksIt()
    {
        var (account, card) = NewCard();

        var result = account.BlockCard(card.Id);

        ResultAssert.Success(result);
        Assert.Equal(CardStatus.Blocked, card.Status);
    }

    [Fact]
    public void BlockCard_AlreadyBlockedCard_ReturnsAlreadyBlocked()
    {
        var (account, card) = NewCard();
        account.BlockCard(card.Id);

        var result = account.BlockCard(card.Id);

        ResultAssert.Failure(result, "Card.AlreadyBlocked");
        Assert.Equal(CardStatus.Blocked, card.Status);
    }

    [Fact]
    public void BlockCard_UnknownCard_ReturnsNotFound()
    {
        var (account, _) = NewCard();

        var result = account.BlockCard(Guid.NewGuid());

        ResultAssert.Failure(result, "Card.NotFound");
    }

    [Fact]
    public void UnblockCard_BlockedCard_ActivatesIt()
    {
        var (account, card) = NewCard();
        account.BlockCard(card.Id);

        var result = account.UnblockCard(card.Id);

        ResultAssert.Success(result);
        Assert.Equal(CardStatus.Active, card.Status);
    }

    [Fact]
    public void UnblockCard_ActiveCard_ReturnsNotBlocked()
    {
        var (account, card) = NewCard();

        var result = account.UnblockCard(card.Id);

        ResultAssert.Failure(result, "Card.NotBlocked");
        Assert.Equal(CardStatus.Active, card.Status);
    }

    [Fact]
    public void UnblockCard_UnknownCard_ReturnsNotFound()
    {
        var (account, _) = NewCard();

        var result = account.UnblockCard(Guid.NewGuid());

        ResultAssert.Failure(result, "Card.NotFound");
    }

    [Fact]
    public void UnblockCard_OnClosedAccount_ReturnsInactiveAndStaysBlocked()
    {
        var (account, card) = NewCard();
        account.BlockCard(card.Id);
        account.Close();

        var result = account.UnblockCard(card.Id);

        ResultAssert.Failure(result, "Account.Inactive");
        Assert.Equal(CardStatus.Blocked, card.Status);
    }
}
