using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MyGameWinTest.src.Model
{
    [TestClass]
    public class ShipTest
    {
        public static readonly ShipName _shipName = ShipName.Tug;
        public Ship _ship;

        [TestInitialize]
        public void ShipInitTest() => _ship = new Ship(_shipName);

        [TestCleanup]
        public void ShipCleanup() => _ship = null;

        [TestMethod]
        public void ShipNameTest() => Assert.AreEqual(_ship.Name, "Tug");

        [TestMethod]
        public void ShipSizeTest() => Assert.AreEqual(_ship.Size, (int) ShipName.Tug);

        [TestMethod]
        public void ShipHitsTakenTest() => Assert.AreEqual(_ship.Hits, 0);

        [TestMethod]
        public void ShipColTest() => Assert.AreEqual(_ship.Column, 0);

        [TestMethod]
        public void ShipRowTest() => Assert.AreEqual(_ship.Row, 0);

        [TestMethod]
        public void ShipDirectionTest() => Assert.AreEqual(_ship.Direction, Direction.LeftRight);

        [TestMethod]
        public void ShipHitTest()
        {
            _ship.Hit();
            Assert.AreEqual(_ship.Hits, 1);
        }

        [TestMethod]
        public void ShipIsDeployedTest() => Assert.IsFalse(_ship.IsDeployed);

        [TestMethod]
        public void ShipIsDestroyedTest()
        {
            _ship = new Ship(_shipName);
            Assert.IsFalse(_ship.IsDestroyed);
            _ship.Hit();
            Assert.IsTrue(_ship.IsDestroyed);
        }
    }
}
