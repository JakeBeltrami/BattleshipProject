using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyGameWinTest.src.Model
{
    [TestClass]
    public class TileTest
    {
        public static readonly Ship _testShip = new Ship(ShipName.Tug);
        public static readonly int _row = 1;
        public static readonly int _col = 1;
        public Tile tile;

        [TestInitialize]
        public void ShipInitTest() => tile = new Tile(_row, _col, _testShip);

        [TestCleanup]
        public void ShipCleanup() => tile = null;

        [TestMethod]
        public void TileClearShipTest()
        {
            tile = new Tile(_row, _col, _testShip);
            tile.ClearShip();

            //Check to see if the set variables are correct.
            Assert.AreEqual(tile.Ship, null);

            //reset the class.
            tile = new Tile(_row, _col, _testShip);
        }


        [TestMethod]
        public void TileViewTest()
        {
            tile = new Tile(_row, _col, _testShip);

            //the ship has been assigned, so it needs to be there.
            Assert.AreEqual(tile.View, TileView.Ship);

            //simulate the ship has been shot.
            tile.Shot = true;

            //the ship has been assigned and it's been hit
            Assert.AreEqual(tile.View, TileView.Hit);

            //clear the ship status.
            tile.ClearShip();

            //Check to see if the set variables are correct.
            Assert.AreEqual(tile.Ship, null);

            //There is no ship and the tile has been shot.
            Assert.AreEqual(tile.View, TileView.Miss);

            //reset the shot variable.
            tile.Shot = false;

            //there shouldn't be a ship at all.
            Assert.AreEqual(tile.View, TileView.Sea);

            //reset the tile class.
            tile = new Tile(_row, _col, _testShip);
        }

        [TestMethod]
        public void TileMatchTest() => Assert.IsTrue(tile.Match(_row,_col));

        [TestMethod]
        public void TileShipTest()
        {
            Tile tile = new Tile(_row, _col, _testShip);

            //Check to see if the ship Exists.
            Assert.AreEqual(tile.Ship, _testShip);

            try
            {
                //try setting off the exception.
                tile.Ship = _testShip;
                //this should throw an exception
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }

            //make the ship null.
            tile.ClearShip();

            //Check to see if the set variables are correct.
            Assert.AreEqual(tile.Ship, null);

            //Re-Assign the ship
            tile.Ship = _testShip;

            //Check to see if the ship Exists.
            Assert.AreEqual(tile.Ship, _testShip);

            //reset the class
            tile = new Tile(_row, _col, _testShip);
        }

        [TestMethod]
        public void TileRowTest() => Assert.AreEqual(tile.Row, _row);

        [TestMethod]
        public void TileColumnTest() => Assert.AreEqual(tile.Column, _col);

        [TestMethod]
        public void TileShotTest()
        {
            Tile tile = new Tile(_row, _col, _testShip);

            //Check to see if the shot matches default.
            Assert.IsFalse(tile.Shot);

            //set false to true.
            tile.Shot = true;

            //check to see if it matches true.
            Assert.IsTrue(tile.Shot);
        }
    }
}
