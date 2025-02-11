using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using NUnit.Framework;
using System.Linq;
using static PairMatchingGame;

namespace SampleApp.Tests;

[TestFixture]
public class PairMatchingGameTests
{
    private Bunit.TestContext _testContext;

    [SetUp]
    public void Setup()
    {
        _testContext = new Bunit.TestContext();
    }

    [TearDown]
    public void TearDown()
    {
        _testContext.Dispose();
    }

    [Test]
    public void CardModel_Initialization_SetsDefaultValues()
    {
        // Arrange
        var card = new CardModel();

        // Assert
        Assert.That(card.Value, Is.EqualTo(""));
        Assert.That(card.IsFlipped, Is.False);
        Assert.That(card.IsMatched, Is.False);
    }

    [Test]
    public void InitializeGame_CreatesCorrectNumberOfCards()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();

        // Act
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;

        // Assert
        Assert.That(cards, Is.Not.Null);
        Assert.That(cards.Count, Is.EqualTo(16)); // 8 pairs = 16 cards
    }

    [Test]
    public void InitializeGame_CardsAreShuffled()
    {
        // Arrange
        var component1 = _testContext.RenderComponent<PairMatchingGame>();
        var component2 = _testContext.RenderComponent<PairMatchingGame>();

        // Act
        component1.Instance.InitializeGame();
        component2.Instance.InitializeGame();

        var cards1 = component1.Instance.GetType().GetProperty("Cards")?.GetValue(component1.Instance, null) as List<CardModel>;
        var cards2 = component2.Instance.GetType().GetProperty("Cards")?.GetValue(component2.Instance, null) as List<CardModel>;

        // Assert
        Assert.That(cards1, Is.Not.Null);
        Assert.That(cards2, Is.Not.Null);

        // Check if the cards are shuffled by comparing the first few elements
        // This is not a perfect test for shuffling, but it's a reasonable check.
        bool areSame = true;
        for (int i = 0; i < 5; i++) // Check the first 5 elements
        {
            if (cards1[i].Value != cards2[i].Value)
            {
                areSame = false;
                break;
            }
        }
        Assert.That(areSame, Is.False, "The cards were not shuffled");
    }

    [Test]
    public void HandleCardClick_FirstCardSelected_FlipsCard()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;
        var card = cards.First(); // Get the first card

        // Act
        component.InvokeAsync(() => component.Instance.HandleCardClick(card));

        // Assert
        Assert.That(card.IsFlipped, Is.True);
    }

    [Test]
    public async Task HandleCardClick_MatchingCards_CardsRemainFlipped()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;

        // Find two cards with the same value
        var firstCard = cards.First();
        var secondCard = cards.FirstOrDefault(c => c.Value == firstCard.Value && c != firstCard);

        // Act
        await component.InvokeAsync(() => component.Instance.HandleCardClick(firstCard));
        await component.InvokeAsync(() => component.Instance.HandleCardClick(secondCard));

        // Assert
        Assert.That(firstCard.IsFlipped, Is.True);
        Assert.That(secondCard.IsFlipped, Is.True);
        Assert.That(firstCard.IsMatched, Is.True);
        Assert.That(secondCard.IsMatched, Is.True);
    }

    [Test]
    public async Task HandleCardClick_NonMatchingCards_CardsFlipBack()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;

        // Find two cards with different values
        var firstCard = cards.First();
        var secondCard = cards.FirstOrDefault(c => c.Value != firstCard.Value);
        if (secondCard == null)
        {
            Assert.Fail("Could not find two cards with different values.");
            return;
        }

        // Act
        await component.InvokeAsync(() => component.Instance.HandleCardClick(firstCard));
        await component.InvokeAsync(() => component.Instance.HandleCardClick(secondCard));

        // Assert
        //Cards are not flipped immediately
        Assert.That(firstCard.IsFlipped, Is.True);
        Assert.That(secondCard.IsFlipped, Is.True);

        //Wait for the cards to flip back
        await Task.Delay(1500);

        Assert.That(firstCard.IsFlipped, Is.False);
        Assert.That(secondCard.IsFlipped, Is.False);
        Assert.That(firstCard.IsMatched, Is.False);
        Assert.That(secondCard.IsMatched, Is.False);
    }

    [Test]
    public void RestartGame_ResetsGameState()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;
        var firstCard = cards.First();
        component.InvokeAsync(() => component.Instance.HandleCardClick(firstCard)); // Flip a card

        // Act
        component.Instance.RestartGame();
        var resetCards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;

        // Assert
        Assert.That(resetCards.All(c => !c.IsFlipped));
        Assert.That(resetCards.All(c => !c.IsMatched));
        Assert.That(component.Instance.GetType().GetProperty("MoveCount")?.GetValue(component.Instance, null), Is.EqualTo(0));
        Assert.That(component.Instance.GetType().GetProperty("ShowWinMessage")?.GetValue(component.Instance, null), Is.EqualTo(false));
    }

    [Test]
    public void HandleCardClick_AlreadyFlippedCard_DoesNothing()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;
        var card = cards.First(); // Get the first card

        // Act
        component.InvokeAsync(() => component.Instance.HandleCardClick(card));
        var isFlippedFirst = card.IsFlipped;
        component.InvokeAsync(() => component.Instance.HandleCardClick(card));
        var isFlippedSecond = card.IsFlipped;

        // Assert
        Assert.That(isFlippedFirst, Is.True);
        Assert.That(isFlippedSecond, Is.True);
    }

    [Test]
    public void HandleCardClick_AlreadyMatchedCard_DoesNothing()
    {
        // Arrange
        var component = _testContext.RenderComponent<PairMatchingGame>();
        component.Instance.InitializeGame();
        var cards = component.Instance.GetType().GetProperty("Cards")?.GetValue(component.Instance, null) as List<CardModel>;

        // Find two cards with the same value
        var firstCard = cards.First();
        var secondCard = cards.FirstOrDefault(c => c.Value == firstCard.Value && c != firstCard);

        // Act
        component.InvokeAsync(() => component.Instance.HandleCardClick(firstCard));
        component.InvokeAsync(() => component.Instance.HandleCardClick(secondCard));
        var isMatchedFirst = firstCard.IsMatched;
        component.InvokeAsync(() => component.Instance.HandleCardClick(firstCard));
        var isMatchedSecond = firstCard.IsMatched;

        // Assert
        Assert.That(isMatchedFirst, Is.True);
        Assert.That(isMatchedSecond, Is.True);
    }
}