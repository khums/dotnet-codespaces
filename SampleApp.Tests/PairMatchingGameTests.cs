using Microsoft.AspNetCore.Components;
using FrontEnd.Pages;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Bunit;


namespace MemoryGameTests
{
    public class PairMatchingGameTests
    {
        private PairMatchingGame component;

        [SetUp]
        public void Setup()
        {
            component = new PairMatchingGame();
            component.JSRuntime = new MockJSRuntime(); // Mock IJSRuntime
            component.OnInitialized();
        }

        [Test]
        public void InitializeGame_CreatesCards()
        {
            Assert.That(component.cards, Is.Not.Null);
            Assert.That(component.cards.Count, Is.EqualTo(16)); // Ensure 8 pairs (16 cards)
        }

        [Test]
        public void InitializeGame_CardsAreShuffled()
        {
            var initialCardValues = component.cards.Select(c => c.Value).ToList();
            component.InitializeGame();
            var newCardValues = component.cards.Select(c => c.Value).ToList();

            Assert.That(initialCardValues, Is.Not.EqualTo(newCardValues)); //cards are shuffled after initialization
        }

        [Test]
        public async Task HandleCardClick_FirstCardSelected()
        {
            var card = component.cards.First();
            await component.HandleCardClick(card);
            Assert.That(card.IsFlipped, Is.True);
        }

        [Test]
        public async Task HandleCardClick_MatchingCards()
        {
            //Find a pair of matching cards
            int matchingValue = component.cards[0].Value;
            PairMatchingGame.CardData card1 = component.cards[0];
            PairMatchingGame.CardData card2 = component.cards.FirstOrDefault(c => c.Value == matchingValue && c != card1);

            Assert.IsNotNull(card2, "Could not find a matching card");

            await component.HandleCardClick(card1);
            await component.HandleCardClick(card2);

            Assert.That(card1.IsMatched, Is.True);
            Assert.That(card2.IsMatched, Is.True);
        }

        [Test]
        public async Task HandleCardClick_NonMatchingCards()
        {
            //Find a pair of non-matching cards
            PairMatchingGame.CardData card1 = component.cards[0];
            PairMatchingGame.CardData card2 = component.cards.FirstOrDefault(c => c.Value != card1.Value);

            Assert.IsNotNull(card2, "Could not find a non-matching card");

            await component.HandleCardClick(card1);
            await component.HandleCardClick(card2);

            Assert.That(card1.IsFlipped, Is.False);
            Assert.That(card2.IsFlipped, Is.False);
        }

        [Test]
        public async Task HandleCardClick_AlreadyFlippedCard()
        {
            var card = component.cards.First();
            card.IsFlipped = true; //Simulate the card being flipped
            bool initialFlippedState = card.IsFlipped;

            await component.HandleCardClick(card);
            Assert.That(card.IsFlipped, Is.EqualTo(initialFlippedState)); // Card should not change its flipped state
        }

        [Test]
        public async Task HandleCardClick_AlreadyMatchedCard()
        {
            var card = component.cards.First();
            card.IsMatched = true; //Simulate the card being matched
            bool initialMatchedState = card.IsMatched;

            await component.HandleCardClick(card);
            Assert.That(card.IsMatched, Is.EqualTo(initialMatchedState)); // Card should not change its matched state
        }

        [Test]
        public void RestartGame_ResetsGameState()
        {
            //Arrange: flip some cards and increment move count
            var card = component.cards.First();
            component.HandleCardClick(card);
            component.moveCount = 5;

            //Act: restart the game
            component.RestartGame();

            //Assert: check that the game state is reset
            Assert.That(component.cards.All(c => !c.IsFlipped && !c.IsMatched), Is.True); //All cards are face down
            Assert.That(component.moveCount, Is.EqualTo(0)); //Move count is reset
            Assert.That(component.gameOver, Is.False); //Game over is reset
        }

        [Test]
        public void CheckForGameOver_GameNotOver()
        {
            //Arrange: ensure not all cards are matched
            component.cards.First().IsMatched = false;

            //Act: check for game over
            component.CheckForGameOver();

            //Assert: game over should be false
            Assert.That(component.gameOver, Is.False);
        }

        [Test]
        public void CheckForGameOver_GameOver()
        {
            //Arrange: set all cards to matched
            foreach (var card in component.cards)
            {
                card.IsMatched = true;
            }

            //Act: check for game over
            component.CheckForGameOver();

            //Assert: game over should be true
            Assert.That(component.gameOver, Is.True);
        }

        //Mock class for IJSRuntime for testing purposes
        public class MockJSRuntime : IJSRuntime
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object[] args)
            {
                // Simulate JavaScript interop, if necessary for specific tests
                return ValueTask.FromResult<TValue>(default);
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, params object[] args)
            {
                // Simulate JavaScript interop with cancellation token, if necessary for specific tests
                return ValueTask.FromResult<TValue>(default);
            }
        }
    }
}