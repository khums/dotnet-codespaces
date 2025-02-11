using Bunit;
using Microsoft.Extensions.DependencyInjection;
using FrontEnd.Pages;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MemoryGameTests
{
    [TestFixture]
    public class PairMatchingGameTests
    {
        private Bunit.TestContext _testContext;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _testContext = new Bunit.TestContext();
            _testContext.Services.AddSingleton<IJSRuntime, MockJSRuntime>();
        }

        [SetUp]
        public void Setup()
        {
            _testContext.JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testContext.Dispose();
        }

        [Test]
        public void InitializeGame_CreatesCards()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            Assert.That(gameInstance.cards, Is.Not.Null);
            Assert.That(gameInstance.cards.Count, Is.EqualTo(16));
        }

        [Test]
        public async Task HandleCardClick_FirstCardSelected()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var card = gameInstance.cards.First();
            await gameInstance.HandleCardClick(card);
            Assert.That(card.IsFlipped, Is.True);
        }

        [Test]
        public async Task HandleCardClick_MatchingCards()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var matchingCards = gameInstance.cards.GroupBy(c => c.Value).First().ToList();
            await gameInstance.HandleCardClick(matchingCards[0]);
            await gameInstance.HandleCardClick(matchingCards[1]);
            Assert.That(matchingCards[0].IsMatched, Is.True);
            Assert.That(matchingCards[1].IsMatched, Is.True);
        }

        [Test]
        public async Task HandleCardClick_NonMatchingCards()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var nonMatchingCards = gameInstance.cards.GroupBy(c => c.Value).SelectMany(g => g).ToList();
            nonMatchingCards[0].Value=10;
            nonMatchingCards[0].Value=12;
            await gameInstance.HandleCardClick(nonMatchingCards[0]);
            await gameInstance.HandleCardClick(nonMatchingCards[1]);
            await Task.Delay(5000);
            Assert.That(nonMatchingCards[0].IsFlipped, Is.False);
            Assert.That(nonMatchingCards[1].IsFlipped, Is.False);
        }

        [Test]
        public async Task HandleCardClick_PreventClick()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var cards = gameInstance.cards.Take(3).ToList();
            cards[0].Value = 1;
            cards[1].Value = 2;
            cards[2].Value = 5;
            
            gameInstance.preventClick=true;
            
            await gameInstance.HandleCardClick(cards[2]);
            Assert.That(cards[2].IsFlipped, Is.False);
        }

        [Test]
        public async Task HandleCardClick_IncrementsMoveCount()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var cards = gameInstance.cards.Take(2).ToList();
            int initialMoveCount = gameInstance.moveCount;
            await gameInstance.HandleCardClick(cards[0]);
            await gameInstance.HandleCardClick(cards[1]);
            Assert.That(gameInstance.moveCount, Is.EqualTo(initialMoveCount + 1));
        }

        [Test]
        public void RestartGame_ResetsGameState()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            gameInstance.cards.ForEach(c => c.IsMatched = true);
            gameInstance.moveCount = 5;
            gameInstance.RestartGame();
            Assert.That(gameInstance.cards.All(c => !c.IsFlipped && !c.IsMatched), Is.True);
            Assert.That(gameInstance.moveCount, Is.EqualTo(0));
        }

        [Test]
        public void CheckForGameOver_GameOver()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            gameInstance.cards.ForEach(c => c.IsMatched = true);
            gameInstance.CheckForGameOver();
            Assert.That(gameInstance.gameOver, Is.True);
        }

        [Test]
        public async Task HandleCardClick_StateHasChangedOnUIThread()
        {
            using var component = _testContext.RenderComponent<PairMatchingGame>();
            var gameInstance = component.Instance;
            var card = gameInstance.cards.First();
            await gameInstance.HandleCardClick(card);
    
            // Re-render the component to reflect state changes
            component.Render();
            
            Assert.That(card.IsFlipped, Is.True);
            
            Assert.That(component.Markup, Does.Contain(card.Value.ToString())); // Check if the flipped card is displayed
        }

        private class MockJSRuntime : IJSRuntime
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
            {
                return ValueTask.FromResult(default(TValue));
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
            {
                return ValueTask.FromResult(default(TValue));
            }
        }
    }
}
