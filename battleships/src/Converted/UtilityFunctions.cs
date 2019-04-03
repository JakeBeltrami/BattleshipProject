/// <summary>
///  This includes a number of utility methods for
///  drawing and interacting with the Mouse.
/// </summary>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

static class UtilityFunctions
{
    public const static int FieldTop = 122;
    public const static int FieldLeft = 349;
    public const static int FieldWidth = 418;
    public const static int FieldHeight = 418;

    public const static int MessageTop = 548;

    public const static int CellWidth = 40;
    public const static int CellHeight = 40;
    public const static int CellGap = 2;

    public const static int ShipGap = 3;

    private readonly static Color SmallSea = SwinGame.RGBAColor(6, 60, 94, 255);
    private readonly static Color SmallShip = Color.Gray;
    private readonly static Color SmallMiss = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color SmallHit = SwinGame.RGBAColor(169, 24, 37, 255);

    private readonly static Color LargeSea = SwinGame.RGBAColor(6, 60, 94, 255);
    private readonly static Color LargeShip = Color.Gray;
    private readonly static Color LargeMiss = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color LargeHit = SwinGame.RGBAColor(252, 2, 3, 255);

    private readonly static Color OutlineColour = SwinGame.RGBAColor(5, 55, 88, 255);
    private readonly static Color ShipFillColour = Color.Gray;
    private readonly static Color ShipOutlineColour = Color.White;
    private readonly static Color MessageColour = SwinGame.RGBAColor(2, 167, 252, 255);

    public const static int AnimationCells = 7;
    public const static int FramesPerCell = 8;

    /// <summary>
    /// Determines if the mouse is in a given rectangle.
    /// </summary>
    /// <param name="x">the x location to check</param>
    /// <param name="y">the y location to check</param>
    /// <param name="w">the width to check</param>
    /// <param name="h">the height to check</param>
    /// <returns>true if the mouse is in the area checked</returns>
    public static bool IsMouseInRectangle(int x, int y, int w, int h)
    {
        Point2D mouse;
        bool result = false;

        mouse = SwinGame.MousePosition();

        // if the mouse is inline with the button horizontally
        if (mouse.X >= x & mouse.X <= x + w)
        {
            // Check vertical position
            if (mouse.Y >= y & mouse.Y <= y + h)
                result = true;
        }

        return result;
    }

    /// <summary>
    /// Draws a large field using the grid and the indicated player's ships.
    /// </summary>
    /// <param name="grid">the grid to draw</param>
    /// <param name="thePlayer">the players ships to show</param>
    /// <param name="showShips">indicates if the ships should be shown</param>
    public static void DrawField(ISeaGrid grid, Player thePlayer, bool showShips)
    {
        DrawCustomField(grid, thePlayer, false, showShips, FieldLeft, FieldTop, FieldWidth, FieldHeight, CellWidth, cellHeight, CellGap);
    }

    /// <summary>
    /// Draws a small field, showing the attacks made and the locations of the player's ships
    /// </summary>
    /// <param name="grid">the grid to show</param>
    /// <param name="thePlayer">the player to show the ships of</param>
    public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
    {
        const int SmallFieldLeft = 39;
        const int SmallFieldTop = 373;
        const int SmallFieldWidth = 166;
        const int SmallFieldHeight = 166;
        const int SmallFieldCellWidth = 13;
        const int SmallFieldCellHeight = 13;
        const int SmallFieldCellGap = 4;

        DrawCustomField(grid, thePlayer, true, true, SmallFieldLeft, SmallFieldTop, SmallFieldWidth, SmallFieldHeight, SmallFieldCellWidth, SmallFieldCellHeight, SmallFieldCellGap);
    }

    /// <summary>
    /// Draws the player's grid and ships.
    /// </summary>
    /// <param name="grid">the grid to show</param>
    /// <param name="thePlayer">the player to show the ships of</param>
    /// <param name="small">true if the small grid is shown</param>
    /// <param name="showShips">true if ships are to be shown</param>
    /// <param name="left">the left side of the grid</param>
    /// <param name="top">the top of the grid</param>
    /// <param name="width">the width of the grid</param>
    /// <param name="height">the height of the grid</param>
    /// <param name="cellWidth">the width of each cell</param>
    /// <param name="cellHeight">the height of each cell</param>
    /// <param name="cellGap">the gap between the cells</param>
    private static void DrawCustomField(ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight, int cellGap)
    {
        // SwinGame.FillRectangle(Color.Blue, left, top, width, height)

        int rowTop;
        int colLeft;

        // Draw the grid
        for (int row = 0; row <= 9; row++)
        {
            rowTop = top + (cellGap + cellHeight) * row;

            for (int column = 0; column <= 9; column++)
            {
                colLeft = left + (cellGap + cellWidth) * column;

                Color fillColor;
                bool draw;

                draw = true;

                switch (grid.Item(row, column))
                {
                    case object _ when TileView.Ship:
                        {
                            draw = false;
                            break;
                        }

                    case object _ when TileView.Miss:
                        {
                            if (small)
                                fillColor = SmallMiss;
                            else
                                fillColor = LargeMiss;
                            break;
                        }

                    case object _ when TileView.Hit:
                        {
                            if (small)
                                fillColor = SmallHit;
                            else
                                fillColor = LargeHit;
                            break;
                        }

                    case object _ when TileView.Sea:
                    case object _ when TileView.Ship:
                        {
                            if (small)
                                fillColor = SmallSea;
                            else
                                draw = false;
                            break;
                        }
                }

                if (draw)
                {
                    SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
                    if (!small)
                        SwinGame.DrawRectangle(OutlineColour, colLeft, rowTop, cellWidth, cellHeight);
                }
            }
        }

        if (!showShips)
            return;

        int shipHeight, shipWidth;
        string shipName;

        // Draw the ships
        foreach (Ship s in thePlayer)
        {
            if (s == null || !s.IsDeployed)
                continue;
            rowTop = top + (cellGap + cellHeight) * s.Row + ShipGap;
            colLeft = left + (cellGap + cellWidth) * s.Column + ShipGap;

            if (s.Direction == Direction.LeftRight)
            {
                shipName = "ShipLR" + s.Size;
                shipHeight = cellHeight - (ShipGap * 2);
                shipWidth = (cellWidth + cellGap) * s.Size - (ShipGap * 2) - cellGap;
            }
            else
            {
                // Up down
                shipName = "ShipUD" + s.Size;
                shipHeight = (cellHeight + cellGap) * s.Size - (ShipGap * 2) - cellGap;
                shipWidth = cellWidth - (ShipGap * 2);
            }

            if (!small)
                SwinGame.DrawBitmap(GameImage(shipName), colLeft, rowTop);
            else
            {
                SwinGame.FillRectangle(ShipFillColour, colLeft, rowTop, shipWidth, shipHeight);
                SwinGame.DrawRectangle(ShipOutlineColour, colLeft, rowTop, shipWidth, shipHeight);
            }
        }
    }

    private static string _message;

    /// <summary>
    /// The message to display
    /// </summary>
    /// <value>The message to display</value>
    /// <returns>The message to display</returns>
    public static string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }

    /// <summary>
    /// Draws the message to the screen
    /// </summary>
    public static void DrawMessage()
    {
        SwinGame.DrawText(Message, MessageColour, GameFont("Courier"), FieldLeft, MessageTop);
    }

    /// <summary>
    /// Draws the background for the current state of the game
    /// </summary>
    public static void DrawBackground()
    {
        switch (CurrentState)
        {
            case object _ when GameState.ViewingMainMenu:
            case object _ when GameState.ViewingGameMenu:
            case object _ when GameState.AlteringSettings:
            case object _ when GameState.ViewingHighScores:
                {
                    SwinGame.DrawBitmap(GameImage("Menu"), 0, 0);
                    break;
                }

            case object _ when GameState.Discovering:
            case object _ when GameState.EndingGame:
                {
                    SwinGame.DrawBitmap(GameImage("Discovery"), 0, 0);
                    break;
                }

            case object _ when GameState.Deploying:
                {
                    SwinGame.DrawBitmap(GameImage("Deploy"), 0, 0);
                    break;
                }

            default:
                {
                    SwinGame.ClearScreen();
                    break;
                }
        }

        SwinGame.DrawFramerate(675, 585);
    }

    public static void AddExplosion(int row, int col)
    {
        AddAnimation(row, col, "Splash");
    }

    public static void AddSplash(int row, int col)
    {
        AddAnimation(row, col, "Splash");
    }

    private static List<Sprite> _Animations = new List<Sprite>();

    private static void AddAnimation(int row, int col, string image)
    {
        Sprite s;
        Bitmap imgObj;

        imgObj = GameImage(image);
        imgObj.SetCellDetails(40, 40, 3, 3, 7);

        AnimationScript animation;
        animation = SwinGame.LoadAnimationScript("splash.txt");

        s = SwinGame.CreateSprite(imgObj, animation);
        s.X = FieldLeft + col * (CellWidth + CellGap);
        s.Y = FieldTop + row * (CellHeight + CellGap);

        s.StartAnimation("splash");
        _Animations.Add(s);
    }

    public static void UpdateAnimations()
    {
        List<Sprite> ended = new List<Sprite>();
        foreach (Sprite s in _Animations)
        {
            SwinGame.UpdateSprite(s);
            if (s.animationHasEnded)
                ended.Add(s);
        }

        foreach (Sprite s in ended)
        {
            _Animations.Remove(s);
            SwinGame.FreeSprite(s);
        }
    }

    public static void DrawAnimations()
    {
        foreach (Sprite s in _Animations)
            SwinGame.DrawSprite(s);
    }

    public static void DrawAnimationSequence()
    {
        int i;
        for (i = 1; i <= AnimationCells * FramesPerCell; i++)
        {
            UpdateAnimations();
            DrawScreen();
        }
    }
}
