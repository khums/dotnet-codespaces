using Microsoft.VisualStudio.TestTools.UnitTesting;
using MontyHall;
using System;

namespace MontyHallTests
{
    [TestClass]
    public class MontyHallTests
    {
        [TestMethod]
        public void InitializeGame_ShouldCreateThreeDoors()
        {
            // Arrange
            var montyHall = new MontyHall();

            // Act
            montyHall.InitializeGame();

            // Assert
            Assert.AreEqual(3, montyHall.Doors.Count);
        }

		[TestMethod]
		public void SelectDoor_Should_Throw_Exception_When_DoorId_Is_Negative()
		{
			// Arrange
			var montyHall = new MontyHall();
			montyHall.InitializeGame();

			// Act & Assert
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => montyHall.SelectDoor(-1));
		}

		[TestMethod]
		public void SelectDoor_Should_Throw_Exception_When_DoorId_Is_Out_Of_Range()
		{
			// Arrange
			var montyHall = new MontyHall();
			montyHall.InitializeGame();

			// Act & Assert
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => montyHall.SelectDoor(3));
		}

        [TestMethod]
        public void SelectDoor_ShouldSetSelectedDoorIdAndIsDoorSelected()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();

            // Act
            montyHall.SelectDoor(1);

            // Assert
            Assert.AreEqual(1, montyHall.SelectedDoorId);
            Assert.IsTrue(montyHall.IsDoorSelected);
        }

        [TestMethod]
        public void RevealGoatDoor_ShouldRevealADoorThatIsNotSelectedOrWinningDoor()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();
			montyHall.WinningDoorId = 0;  //Force winning door to 0 for testability
            montyHall.SelectDoor(1);

            // Act
            montyHall.RevealGoatDoor();

            // Assert
            Assert.AreNotEqual(-1, montyHall.RevealedGoatDoorId);
            Assert.AreNotEqual(1, montyHall.RevealedGoatDoorId); // Not the selected door
            Assert.AreNotEqual(0, montyHall.RevealedGoatDoorId); // Not the winning door (forced to be door 0)
            Assert.IsTrue(montyHall.Doors[montyHall.RevealedGoatDoorId].IsRevealed);
        }

        [TestMethod]
        public void SwitchDoor_ShouldChangeSelectedDoor()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();
			montyHall.WinningDoorId = 0; //force car to door 0 for testability
            montyHall.SelectDoor(1);
            montyHall.RevealGoatDoor();
            int originalSelectedDoor = montyHall.SelectedDoorId;

            // Act
            montyHall.SwitchDoor();

            // Assert
            Assert.AreNotEqual(originalSelectedDoor, montyHall.SelectedDoorId);
            Assert.IsTrue(montyHall.DidSwitch);
        }

        [TestMethod]
        public void StickWithDoor_ShouldNotChangeSelectedDoor()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();
			montyHall.WinningDoorId = 0; //force car to door 0 for testability
            montyHall.SelectDoor(1);
            montyHall.RevealGoatDoor();
            int originalSelectedDoor = montyHall.SelectedDoorId;

            // Act
            montyHall.StickWithDoor();

            // Assert
            Assert.AreEqual(originalSelectedDoor, montyHall.SelectedDoorId);
            Assert.IsFalse(montyHall.DidSwitch);
        }

        [TestMethod]
        public void DetermineWinner_ShouldSetPlayerWonCorrectlyWhenSwitching()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();
			montyHall.WinningDoorId = 0; //force car to door 0 for testability
            montyHall.SelectDoor(1);
            montyHall.RevealGoatDoor();
            montyHall.SwitchDoor();

            // Act
            montyHall.DetermineWinner();

            // Assert
            Assert.IsTrue(montyHall.PlayerWon); // Switching should win in this forced scenario
        }

        [TestMethod]
        public void DetermineWinner_ShouldSetPlayerWonCorrectlyWhenSticking()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.InitializeGame();
			montyHall.WinningDoorId = 1; //force car to door 1 for testability
            montyHall.SelectDoor(1);
            montyHall.RevealGoatDoor();
            montyHall.StickWithDoor();

            // Act
            montyHall.DetermineWinner();

            // Assert
            Assert.IsTrue(montyHall.PlayerWon); // Sticking should win in this forced scenario
        }

        [TestMethod]
        public void ResetStatistics_ShouldResetAllStatistics()
        {
            // Arrange
            var montyHall = new MontyHall();
            montyHall.WinsStick = 5;
            montyHall.LossesStick = 3;
            montyHall.WinsSwitch = 7;
            montyHall.LossesSwitch = 2;

            // Act
            montyHall.ResetStatistics();

            // Assert
            Assert.AreEqual(0, montyHall.WinsStick);
            Assert.AreEqual(0, montyHall.LossesStick);
            Assert.AreEqual(0, montyHall.WinsSwitch);
            Assert.AreEqual(0, montyHall.LossesSwitch);
        }
    }
}