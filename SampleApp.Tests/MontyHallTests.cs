using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using NUnit.Framework;
using System;
using System.Linq;
using FrontEnd.Pages;

namespace MontyHallTests
{
    public class MontyHallTests
    {
        private TestContext testContext;
        private IRenderedComponent<MontyHall> component;

        [SetUp]
        public void Setup()
        {
            testContext = new TestContext();
            component = testContext.RenderComponent<MontyHall>();
        }

        [Test]
        public void SelectDoor_ValidDoorIndex_UpdatesState()
        {
            // Arrange
            var montyHall = component.Instance;

            // Act
            montyHall.SelectDoor(0);

            // Assert
            Assert.That(montyHall.selectedDoor, Is.EqualTo(0));
            Assert.That(montyHall.Message, Does.Contain("You selected door #1"));
        }

        [Test]
        public void SelectDoor_InvalidDoorIndex_DisplaysErrorMessage()
        {
            // Arrange
            var montyHall = component.Instance;

            // Act
            montyHall.SelectDoor(3);

            // Assert
            Assert.That(montyHall.Message, Is.EqualTo("Invalid door selection."));
        }

        [Test]
        public void SelectDoor_AlreadySelectedDoor_DisplaysErrorMessage()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.SelectDoor(1);

            // Act
            montyHall.SelectDoor(2);

            // Assert
            Assert.That(montyHall.Message, Is.EqualTo("You have already selected a door."));
        }

        [Test]
        public void RevealGoat_NoDoorSelected_DisplaysErrorMessage()
        {
            // Arrange
            var montyHall = component.Instance;

            // Act
            montyHall.RevealGoat();

            // Assert
            Assert.That(montyHall.Message, Is.EqualTo("Select a door first!"));
        }

        [Test]
        public void RevealGoat_ValidSelection_RevealsGoat()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.SelectDoor(0);
            montyHall.carDoor = 1; //force the car to be behind door 1, so a goat MUST be revealed behind door 2

            // Act
            montyHall.RevealGoat();

            // Assert
            Assert.That(montyHall.revealedGoatDoor, Is.EqualTo(2));
            Assert.That(montyHall.Message, Does.Contain("I've revealed a goat behind door #3"));
            Assert.That(montyHall.ShowSwitchButtons, Is.True);
        }

        [Test]
        public void MakeFinalChoice_NoGoatRevealed_DisplaysErrorMessage()
        {
            // Arrange
            var montyHall = component.Instance;

            // Act
            montyHall.MakeFinalChoice(true);

            // Assert
            Assert.That(montyHall.Message, Is.EqualTo("Cannot make a final choice before a goat is revealed."));
        }

        [Test]
        public void MakeFinalChoice_SwitchAndWin_UpdatesStatisticsAndMessage()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.carDoor = 1;
            montyHall.selectedDoor = 0;
            montyHall.revealedGoatDoor = 2;

            // Act
            montyHall.MakeFinalChoice(true);

            // Assert
            Assert.That(montyHall.WinsSwitch, Is.EqualTo(1));
            Assert.That(montyHall.RoundsPlayed, Is.EqualTo(1));
            Assert.That(montyHall.Message, Does.Contain("You switched to door #2 and WON!"));
            Assert.That(montyHall.ShowSwitchButtons, Is.False);
            Assert.That(montyHall.ShowRestartButton, Is.True);
        }

        [Test]
        public void MakeFinalChoice_StickAndLose_UpdatesStatisticsAndMessage()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.carDoor = 1;
            montyHall.selectedDoor = 0;
            montyHall.revealedGoatDoor = 2;

            // Act
            montyHall.MakeFinalChoice(false);

            // Assert
            Assert.That(montyHall.LossesStick, Is.EqualTo(1));
            Assert.That(montyHall.RoundsPlayed, Is.EqualTo(1));
            Assert.That(montyHall.Message, Does.Contain("You stuck with door #1 and LOST."));
            Assert.That(montyHall.ShowSwitchButtons, Is.False);
            Assert.That(montyHall.ShowRestartButton, Is.True);
        }

        [Test]
        public void RestartGame_ResetsGameState()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.SelectDoor(0);
            montyHall.RevealGoat();
            montyHall.MakeFinalChoice(true);

            // Act
            montyHall.RestartGame();

            // Assert
            Assert.That(montyHall.selectedDoor, Is.EqualTo(-1));
            Assert.That(montyHall.revealedGoatDoor, Is.EqualTo(-1));
            Assert.That(montyHall.Message, Is.EqualTo("Choose a door."));
            Assert.That(montyHall.ShowSwitchButtons, Is.False);
            Assert.That(montyHall.ShowRestartButton, Is.False);
        }

        [Test]
        public void ResetStatistics_ResetsStatistics()
        {
            // Arrange
            var montyHall = component.Instance;
            montyHall.WinsStick = 5;
            montyHall.LossesStick = 3;
            montyHall.WinsSwitch = 2;
            montyHall.LossesSwitch = 1;
            montyHall.RoundsPlayed = 11;

            // Act
            montyHall.ResetStatistics();

            // Assert
            Assert.That(montyHall.WinsStick, Is.EqualTo(0));
            Assert.That(montyHall.LossesStick, Is.EqualTo(0));
            Assert.That(montyHall.WinsSwitch, Is.EqualTo(0));
            Assert.That(montyHall.LossesSwitch, Is.EqualTo(0));
            Assert.That(montyHall.RoundsPlayed, Is.EqualTo(0));
        }
    }
}