using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MyGameWinTest.src.Model
{
    [TestClass]
    public class SeaGridTest
    {
        public Dictionary<ShipName, Ship> Ships = new Dictionary<ShipName, Ship>();
        public SeaGrid seaGrid;

        [TestInitialize]
        public void SeaGridInitTest()
        {
            //build an array of ships.
            foreach(ShipName s in System.Enum.GetValues(typeof(ShipName)))
                if(s != ShipName.None)
                    Ships.Add(s, new Ship(s));

            //now add the new seagrid
            seaGrid = new SeaGrid(Ships);
        }

        [TestCleanup]
        public void SeaGridCleanup() => Ships = null;

        [TestMethod]
        public void SeaGridAllDeployedTest() => Assert.IsFalse(seaGrid.AllDeployed);

        [TestMethod]
        public void SeaGridWidthTest() => Assert.AreEqual(seaGrid.Width, 10);

        [TestMethod]
        public void SeaGridHeightTest() => Assert.AreEqual(seaGrid.Height, 10);

        [TestMethod]
        public void SeaGridShipsKilledTest() => Assert.AreEqual(seaGrid.ShipsKilled, 0);
        

        [TestMethod]
        public void SeaGridItemTest()
        {
            seaGrid = new SeaGrid(Ships);
            //the sea grid should be empty. Check to make sure though.
            for (int x = 0; x < seaGrid.Width; x++)
                for(int y = 0; y < seaGrid.Height; y++)
                    Assert.AreEqual(seaGrid.Item(x,y), TileView.Sea);
        }

        [TestMethod]
        public void SeaGridMoveShipTest()
        {
            seaGrid = new SeaGrid(Ships);
            seaGrid.MoveShip(0, 0, ShipName.Tug, Direction.LeftRight);
            Assert.AreEqual(seaGrid.Item(0,0), TileView.Ship);
        }

        [TestMethod]
        public void SeaGridHitTileTest()
        {
            seaGrid = new SeaGrid(Ships);
            seaGrid.MoveShip(0, 0, ShipName.Tug, Direction.LeftRight);
            AttackResult ar = seaGrid.HitTile(0, 0);
            Assert.AreEqual(ar.Value, ResultOfAttack.Destroyed);
        }
    }
}
