using System.Collections.Generic;


/// <summary>
/// A Ship has all the details about itself. For example the shipname,
/// size, number of hits taken and the location. Its able to add tiles,
/// remove, hits taken and if its deployed and destroyed.
/// </summary>
/// <remarks>
/// Deployment information is supplied to allow ships to be drawn.
/// </remarks>
public class Ship
{
    private ShipName shipName;
    private int sizeOfShip;
    private int hitsTaken = 0;
    private List<Tile> tiles;
    private int row;
    private int col;
    private Direction direction;

    /// <summary>
    /// The type of ship
    /// </summary>
    /// <value>The type of ship</value>
    /// <returns>The type of ship</returns>
    public string Name
    {
        get
        {
            if (shipName == ShipName.AircraftCarrier)
                return "Aircraft Carrier";

            return shipName.ToString();
        }
    }

    /// <summary>
    /// The number of cells that this ship occupies.
    /// </summary>
    /// <value>The number of hits the ship can take</value>
    /// <returns>The number of hits the ship can take</returns>
    public int Size => sizeOfShip;

    /// <summary>
    /// The number of hits that the ship has taken.
    /// </summary>
    /// <value>The number of hits the ship has taken.</value>
    /// <returns>The number of hits the ship has taken</returns>
    /// <remarks>When this equals Size the ship is sunk</remarks>
    public int Hits => hitsTaken;

    /// <summary>
    /// The row location of the ship
    /// </summary>
    /// <value>The topmost location of the ship</value>
    /// <returns>the row of the ship</returns>
    public int Row => row;

	/// <summary>
    /// The column location of the ship
    /// </summary>
    public int Column => col;

	/// <summary>
    /// Returns whether the ship is vertical or horizontal
    /// </summary>
    public Direction Direction => direction;

	/// <summary>
    /// Ship constructor, assigns what type of ship it is, what tiles it's on and how large it is
    /// </summary>
    public Ship(ShipName ship)
    {
        shipName = ship;
        tiles = new List<Tile>();

        // gets the ship size from the enumarator
        sizeOfShip = (int) shipName;
    }

    /// <summary>
    /// Add tile adds the ship tile
    /// </summary>
    /// <param name="tile">one of the tiles the ship is on</param>
    public void AddTile(Tile tile)
    {
        tiles.Add(tile);
    }

    /// <summary>
    /// Remove clears the tile back to a sea tile
    /// </summary>
    public void Remove()
    {
        foreach (Tile tile in tiles)
            tile.ClearShip();
        tiles.Clear();
    }

	/// <summary>
    /// Reduces the 'health' of the ship by one
    /// </summary>
    public void Hit()
    {
        hitsTaken = hitsTaken + 1;
    }

    /// <summary>
    /// IsDeployed returns if the ships is deployed, if its deplyed it has more than
    /// 0 tiles
    /// </summary>
    public bool IsDeployed => tiles.Count > 0;
  
	/// <summary>
    /// If the ship's health is 0 (the amount of hits it has taken is equal to its size) it returns true
    /// </summary>
    public bool IsDestroyed => Hits == Size;

    /// <summary>
    /// Record that the ship is now deployed.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    internal void Deployed(Direction direction, int row, int col)
    {
        this.row = row;
        this.col = col;
        this.direction = direction;
    }
}
