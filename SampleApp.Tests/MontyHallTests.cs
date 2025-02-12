using Bunit;
using NUnit.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using FrontEnd.Pages;

[TestFixture]
public class MontyHallTests
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
    public void GameStartsWithThreeDoors()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        var doors = component.FindAll("button").Where(x=> x.Children.Any(span => span.TextContent.Contains("🚪"))).Count();
        Assert.AreEqual(3, doors);
    }

    [Test]
    public void SelectingADoorDisablesFurtherSelection()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        var buttons = component.FindAll("button");
        var doorButtons = buttons.Where(x=> x.Children.Any(span => span.TextContent.Contains("🚪")));
        doorButtons.First().Click();

        // Log button states
        foreach (var btn in doorButtons)
        {
            Console.WriteLine($"Button Disabled: {btn.HasAttribute("disabled")}");
        }

        doorButtons = component.FindAll("button").Where(x=> x.Children.Any(span => span.TextContent.Contains("🚪")
                                            ||span.TextContent.Contains("🐐")));
        Assert.AreEqual(3, doorButtons.Count());
        Assert.IsTrue(doorButtons.All(btn => btn.HasAttribute("disabled")));
    }


    [Test]
    public void SwitchingDoorChangesSelection()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        var doorButtons = component.FindAll("button");
        doorButtons[0].Click();
        component.Find("button:contains('Switch')").Click();
        Assert.IsTrue(component.Instance.DidSwitch);
    }

    [Test]
    public void StickingWithDoorKeepsSelection()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        var doorButtons = component.FindAll("button");
        doorButtons[0].Click();
        component.Find("button:contains('Stick')").Click();
        Assert.IsFalse(component.Instance.DidSwitch);
    }

    [Test]
    public async Task WinningWithStickIncrementsStickWins()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 0;
            component.Instance.SelectDoor(0);
            component.Instance.StickWithDoor();
        });

        Assert.AreEqual(1, component.Instance.WinsStick);
    }


    [Test]
    public async Task LosingWithStickIncrementsStickLosses()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(0);
            component.Instance.StickWithDoor();
        });

        Assert.AreEqual(1, component.Instance.LossesStick);
    }

    [Test]
    public async Task WinningWithSwitchIncrementsSwitchWins()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(0);
            component.Instance.SwitchDoor();
        });
        Assert.AreEqual(1, component.Instance.WinsSwitch);
    }

    [Test]
    public async Task LosingWithSwitchIncrementsSwitchLosses()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(1);
            component.Instance.SwitchDoor();
        });
        Assert.AreEqual(1, component.Instance.LossesSwitch);
    }

    [Test]
    public async Task ResetStatisticsResetsAllWinLossCounters()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinsStick = 5;
            component.Instance.LossesStick = 3;
            component.Instance.WinsSwitch = 4;
            component.Instance.LossesSwitch = 2;
            component.Instance.ResetStatistics();
        });
        Assert.AreEqual(0, component.Instance.WinsStick);
        Assert.AreEqual(0, component.Instance.LossesStick);
        Assert.AreEqual(0, component.Instance.WinsSwitch);
        Assert.AreEqual(0, component.Instance.LossesSwitch);
    }

    [Test]
    public void ClickingSwitchOrStickWithoutSelectingDoorDoesNothing()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        
        // Attempt to find "Switch" or "Stick" before selecting a door
        Assert.Throws<ElementNotFoundException>(() => component.Find("button:contains('Switch')"));
        Assert.Throws<ElementNotFoundException>(() => component.Find("button:contains('Stick')"));
    }

    [Test]
    public async Task RestartGameResetsAllState()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(0);
            component.Instance.SwitchDoor();
            
            component.Find("button:contains('Restart')").Click();
        });
        
        Assert.AreEqual(-1, component.Instance.SelectedDoorId);
        Assert.AreEqual(-1, component.Instance.RevealedGoatDoorId);
        Assert.AreEqual(false, component.Instance.IsDoorSelected);
        Assert.AreEqual(false, component.Instance.IsSwitchOrStickChosen);
        Assert.AreEqual(false, component.Instance.PlayerWon);
    }

    [Test]
    public async Task GoatRevealLogicIsCorrect()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            // Manually set winning door
            component.Instance.WinningDoorId = 2;
            component.Instance.SelectDoor(2);
        });
        
        // Ensure a goat door was revealed
        Assert.IsTrue(component.Instance.RevealedGoatDoorId != 2);
        Assert.IsFalse(component.Instance.Doors[component.Instance.RevealedGoatDoorId].HasCar);
    }

    [Test]
    public async Task UIShowsCorrectWinMessage()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(1);
            component.Instance.StickWithDoor();
        });
        var message = component.Find("p[style*='color: green;']").TextContent;
        Assert.IsTrue(message.Contains("You win!"));
    }

    [Test]
    public async Task UIShowsCorrectLossMessage()
    {
        var component = _testContext.RenderComponent<MontyHall>();
        
        await component.InvokeAsync(() =>
        {
            component.Instance.WinningDoorId = 1;
            component.Instance.SelectDoor(0);
            component.Instance.StickWithDoor();
        });
        var message = component.Find("p[style*='color: red;']").TextContent;
        Assert.IsTrue(message.Contains("You lose!"));
    }

    [Test]
    public async Task RapidClickingDoesNotBreakGame()
    {
        var component = _testContext.RenderComponent<MontyHall>();

        await component.InvokeAsync(async () =>
        {
            for (int i = 0; i < 10; i++) // Simulating rapid clicks
            {
                var doorButtons = component.FindAll("button")
                    .Where(x => x.Children.Any(span => span.TextContent.Contains("🚪")))
                    .ToList();

                if (doorButtons.Any())
                {
                    doorButtons[0].Click(); // Click the first available door
                    component.Render(); // Allow UI to stabilize
                }
            }
        });

        // Ensure only one door is actually selected after rapid clicks
        Assert.AreEqual(1, component.Instance.Doors.Count(d => d.Id == component.Instance.SelectedDoorId && component.Instance.IsDoorSelected));
    }

}
