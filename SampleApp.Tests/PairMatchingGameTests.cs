using NUnit.Framework;
using MemoryGame.Pages; // Replace with your actual namespace
using System.Collections.Generic;
using System.Linq;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryGameTests
{
    public class PairMatchingGameTests : TestContext
    {
        [SetUp]
        public void Setup()
        {
            //Required for JSInterop calls in Blazor
            Services.AddSingleton<IJSRuntime>(new MockJSRuntime());
        }

        [Test]
        public void InitializeGame_CreatesCorrectNumberOfCards()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            int gridSize = (int)component.Instance.GetType().GetField("gridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);
            // Act - Initialization happens in OnInitialized

            // Assert
            Assert.That(cards.Count, Is.EqualTo(gridSize * gridSize)); // Default grid size is 4x4 = 16
        }

        [Test]
        public void InitializeGame_CreatesPairsOfCards()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;

            // Act - Initialization happens in OnInitialized

            // Assert
            var cardValues = cards.Select(c => c.Value).ToList();
            var groupedValues = cardValues.GroupBy(v => v);
            foreach (var group in groupedValues)
            {
                Assert.That(group.Count(), Is.EqualTo(2), $"Value {group.Key} does not have a pair.");
            }
        }

        [Test]
        public async Task HandleCardClick_FirstCardFlip_CardIsFlipped()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int firstCardFlippedIndex = (int)component.Instance.GetType().GetField("firstCardFlippedIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);


            // Act
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] });
            firstCardFlippedIndex = (int)component.Instance.GetType().GetField("firstCardFlippedIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);

            // Assert
            Assert.That(cards[0].IsFlipped, Is.True);
            Assert.That(firstCardFlippedIndex, Is.EqualTo(0));
        }

        [Test]
        public async Task HandleCardClick_SecondCardFlipMatch_CardsRemainFlipped()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //Force same values for cards[0] and cards[1]
            cards[0].Value = 1;
            cards[1].Value = 1;

            // Act
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] });
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[1] });
            //await Task.Delay(1500); //Wait for the delay in HandleCardClick

            // Assert
            Assert.That(cards[0].IsFlipped, Is.True);
            Assert.That(cards[1].IsFlipped, Is.True);
            Assert.That(cards[0].IsMatched, Is.True);
            Assert.That(cards[1].IsMatched, Is.True);
        }

        [Test]
        public async Task HandleCardClick_SecondCardFlipNoMatch_CardsFlipBack()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //Force different values for cards[0] and cards[1]
            cards[0].Value = 1;
            cards[1].Value = 2;

            // Act
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] });
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[1] });
            await Task.Delay(1500); //Wait for the delay in HandleCardClick

            // Assert
            Assert.That(cards[0].IsFlipped, Is.False);
            Assert.That(cards[1].IsFlipped, Is.False);
            Assert.That(cards[0].IsMatched, Is.False);
            Assert.That(cards[1].IsMatched, Is.False);
        }

        [Test]
        public async Task HandleCardClick_SameCardTwice_SecondClickIgnored()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            cards[0].Value = 1;
            cards[1].Value = 2;

            // Act
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] });
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] }); //Click the same card again
            await Task.Delay(1500);

            // Assert
            Assert.That(cards[0].IsFlipped, Is.True);
            int firstCardFlippedIndex = (int)component.Instance.GetType().GetField("firstCardFlippedIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);
            int secondCardFlippedIndex = (int)component.Instance.GetType().GetField("secondCardFlippedIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);

            Assert.That(firstCardFlippedIndex, Is.EqualTo(0));
            Assert.That(secondCardFlippedIndex, Is.EqualTo(-1));
        }

        [Test]
        public async Task RestartGame_ResetsGameState()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var restartGameMethod = component.Instance.GetType().GetMethod("RestartGame", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            cards[0].Value = 1;
            cards[1].Value = 1;
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[0] });
            await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[1] });
            await Task.Delay(1500);
            // Act
            restartGameMethod.Invoke(component.Instance, null);

            // Assert
            cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            int moves = (int)component.Instance.GetType().GetField("moves", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);
            int matchesFound = (int)component.Instance.GetType().GetField("matchesFound", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);
            bool gameWon = (bool)component.Instance.GetType().GetField("gameWon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);

            Assert.That(cards.All(c => !c.IsFlipped && !c.IsMatched));
            Assert.That(moves, Is.EqualTo(0));
            Assert.That(matchesFound, Is.EqualTo(0));
            Assert.That(gameWon, Is.False);
        }

        [Test]
        public async Task WinCondition_AllPairsMatched_GameWonIsTrue()
        {
            // Arrange
            var component = RenderComponent<PairMatchingGame>();
            var cards = component.Instance.GetType().GetField("cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance) as List<Card>;
            var handleCardClickMethod = component.Instance.GetType().GetMethod("HandleCardClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int gridSize = (int)component.Instance.GetType().GetField("gridSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);


            // Force all cards to be matched
            for (int i = 0; i < gridSize * gridSize; i += 2)
            {
                cards[i].Value = i;
                cards[i + 1].Value = i;
                await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[i] });
                await (Task)handleCardClickMethod.Invoke(component.Instance, new object[] { cards[i + 1] });
            }
            await Task.Delay(1500);

            // Act
            bool gameWon = (bool)component.Instance.GetType().GetField("gameWon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);


            // Assert
            Assert.That(gameWon, Is.True);
        }
    }

    //Mock the IJSRuntime for testing purposes
    public class MockJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object[] args)
        {
            // Simulate JavaScript interop calls if needed
            return ValueTask.FromResult<TValue>(default);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, params object[] args)
        {
            return ValueTask.FromResult<TValue>(default);
        }
    }
}