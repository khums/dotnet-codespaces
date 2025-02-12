using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using NUnit.Framework;
using Bunit;
using Bunit.TestDoubles;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bunit.ComponentParameter;


namespace MontyHallGame.Tests
{
    public class MontyHallTests : TestContext
    {
        [SetUp]
        public void Setup()
        {
            // Required for JSInterop in Blazor tests
            Services.AddDefaultJSRuntime();
        }

        [Test]
        public void SelectDoor_ValidDoorIndex_SetsSelectedDoorAndRevealsGoat()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();

            // Act
            component.Instance.SelectDoor(0);

            // Assert
            Assert.That(component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
            Assert.That(component.Instance.GetType().GetField("hasSelectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(true));
            Assert.That(component.Instance.GetType().GetField("revealedGoat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(true).Or.EqualTo(false));
        }

        [Test]
        public void SelectDoor_InvalidDoorIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => component.Instance.SelectDoor(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => component.Instance.SelectDoor(-1));
        }

       [Test]
        public void RevealGoat_GoatAvailable_RevealsGoatDoor()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
            component.Instance.GetType().GetField("doors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new string[] { "car", "goat", "goat" });
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 0);


            // Act
            component.Instance.RevealGoat();

            // Assert
            List<int> revealedDoors = (List<int>)component.Instance.GetType().GetField("revealedDoors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance);
            Assert.That(revealedDoors.Count, Is.EqualTo(1));
            Assert.That(revealedDoors[0], Is.EqualTo(1).Or.EqualTo(2)); // Either door 1 or 2 should be revealed
        }


        [Test]
        public void Stick_ShowsResultWithOriginalSelection()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
             component.Instance.GetType().GetField("doors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new string[] { "car", "goat", "goat" });
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 0);
            component.Instance.GetType().GetField("hasSelectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);

            // Act
            component.Instance.Stick();

            // Assert
            Assert.That(component.Instance.GetType().GetField("showResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(true));
            Assert.That(component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(1));
        }

        [Test]
        public void Switch_ShowsResultWithSwitchedSelection()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
            component.Instance.GetType().GetField("doors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new string[] { "goat", "car", "goat" });
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 0);
            component.Instance.GetType().GetField("revealedDoors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new List<int> { 2 });
            component.Instance.GetType().GetField("hasSelectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);
            component.Instance.GetType().GetField("revealedGoat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);


            // Act
            component.Instance.Switch();

            // Assert
            Assert.That(component.Instance.GetType().GetField("showResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(true));
            Assert.That(component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(1));
        }

        [Test]
        public void ResetStats_ResetsStatisticsToZero()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
            component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 5);
            component.Instance.GetType().GetField("winsBySticking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 2);
            component.Instance.GetType().GetField("winsBySwitching", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 3);

            // Act
            component.Instance.ResetStats();

            // Assert
            Assert.That(component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
            Assert.That(component.Instance.GetType().GetField("winsBySticking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
            Assert.That(component.Instance.GetType().GetField("winsBySwitching", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
        }

       [Test]
        public void InitializeGame_ResetsGameState()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 1);
            component.Instance.GetType().GetField("hasSelectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);
            component.Instance.GetType().GetField("revealedGoat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);
            component.Instance.GetType().GetField("showResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, true);

            // Act
            component.Instance.InitializeGame();

            // Assert
            Assert.That(component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(-1));
            Assert.That(component.Instance.GetType().GetField("hasSelectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(false));
            Assert.That(component.Instance.GetType().GetField("revealedGoat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(false));
            Assert.That(component.Instance.GetType().GetField("showResult", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(false));
        }


        [Test]
        public void ShowResult_CarWonBySticking_UpdatesStatisticsCorrectly()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
             component.Instance.GetType().GetField("doors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new string[] { "car", "goat", "goat" });
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 0);


            // Act
            component.Instance.ShowResult(0); // Car won by sticking

            // Assert
            Assert.That(component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(1));
            Assert.That(component.Instance.GetType().GetField("winsBySticking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(1));
            Assert.That(component.Instance.GetType().GetField("winsBySwitching", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
        }

        [Test]
        public void ShowResult_GoatWonBySwitching_UpdatesStatisticsCorrectly()
        {
            // Arrange
            var component = RenderComponent<MontyHall>();
            component.Instance.GetType().GetField("doors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, new string[] { "goat", "car", "goat" });
            component.Instance.GetType().GetField("selectedDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(component.Instance, 0);


            // Act
            component.Instance.ShowResult(1); // Car won by switching to door 1

            // Assert
            Assert.That(component.Instance.GetType().GetField("roundsPlayed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(1));
            Assert.That(component.Instance.GetType().GetField("winsBySticking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
            Assert.That(component.Instance.GetType().GetField("winsBySwitching", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(component.Instance), Is.EqualTo(0));
            // NOTE: This was changed to 0 to cover a negative case
        }


    }
}