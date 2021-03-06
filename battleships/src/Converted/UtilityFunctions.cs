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
    public const static int Field_Top = 122;
    public const static int Field_Left = 349;
    public const static int Field_Width = 418;
    public const static int Field_Height = 418;

    public const static int Message_Top = 548;

    public const static int Cell_Width = 40;
    public const static int Cell_Height = 40;
    public const static int Cell_Gap = 2;

    public const static int Ship_Gap = 3;

    private readonly static Color Small_Sea = SwinGame.RGBAColor(6, 60, 94, 255);
    private readonly static Color Small_Ship = Color.Gray;
    private readonly static Color Small_Miss = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color Small_Hit = SwinGame.RGBAColor(169, 24, 37, 255);

    private readonly static Color Large_Sea = SwinGame.RGBAColor(6, 60, 94, 255);
    private readonly static Color Large_Ship = Color.Gray;
    private readonly static Color Large_Miss = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color Large_Hit = SwinGame.RGBAColor(252, 2, 3, 255);

    private readonly static Color Outline_Colour = SwinGame.RGBAColor(5, 55, 88, 255);
    private readonly static Color Ship_Fill_Colour = Color.Gray;
    private readonly static Color Ship_Outline_Colour = Color.White;
    private readonly static Color Message_Colour = SwinGame.RGBAColor(2, 167, 252, 255);

    public const static int Animation_Cells = 7;
    public const static int Frames_Per_Cell = 8;

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
        DrawCustomField(grid, thePlayer, false, showShips, Field_Left, Field_Top, Field_Width, Field_Height, Cell_Width, cellHeight, Cell_Gap);
    }

    /// <summary>
    /// Draws a small field, showing the attacks made and the locations of the player's ships
    /// </summary>
    /// <param name="grid">the grid to show</param>
    /// <param name="thePlayer">the player to show the ships of</param>
    public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
    {
        const int Small_Field_Left = 39;
        const int Small_Field_Top = 373;
        const int Small_Field_Width = 166;
        const int Small_Field_Height = 166;
        const int Small_Field_Cell_Width = 13;
        const int Small_Field_Cell_Height = 13;
        const int Small_Field_Cell_Gap = 4;

        DrawCustomField(grid, thePlayer, true, true, Small_Field_Left, Small_Field_Top, Small_Field_Width, Small_Field_Height, Small_Field_Cell_Width, Small_Field_Cell_Height, Small_Field_Cell_Gap);
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
                                fillColor = Small_Miss;
                            else
                                fillColor = Large_Miss;
                            break;
                        }

                    case object _ when TileView.Hit:
                        {
                            if (small)
                                fillColor = Small_Hit;
                            else
                                fillColor = Large_Hit;
                            break;
                        }

                    case object _ when TileView.Sea:
                    case object _ when TileView.Ship:
                        {
                            if (small)
                                fillColor = Small_Sea;
                            else
                                draw = false;
                            break;
                        }
                }

                if (draw)
                {
                    SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
                    if (!small)
                        SwinGame.DrawRectangle(Outline_Colour, colLeft, rowTop, cellWidth, cellHeight);
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
            rowTop = top + (cellGap + cellHeight) * s.Row + Ship_Gap;
            colLeft = left + (cellGap + cellWidth) * s.Column + Ship_Gap;

            if (s.Direction == Direction.LeftRight)
            {
                shipName = "ShipLR" + s.Size;
                shipHeight = cellHeight - (Ship_Gap * 2);
                shipWidth = (cellWidth + cellGap) * s.Size - (Ship_Gap * 2) - cellGap;
            }
            else
            {
                // Up down
                shipName = "ShipUD" + s.Size;
                shipHeight = (cellHeight + cellGap) * s.Size - (Ship_Gap * 2) - cellGap;
                shipWidth = cellWidth - (Ship_Gap * 2);
            }

            if (!small)
                SwinGame.DrawBitmap(GameImage(shipName), colLeft, rowTop);
            else
            {
                SwinGame.FillRectangle(Ship_Fill_Colour, colLeft, rowTop, shipWidth, shipHeight);
                SwinGame.DrawRectangle(Ship_Outline_Colour, colLeft, rowTop, shipWidth, shipHeight);
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
        SwinGame.DrawText(Message, Message_Colour, GameFont("Courier"), Field_Left, Message_Top);
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
        s.X = Field_Left + col * (Cell_Width + Cell_Gap);
        s.Y = Field_Top + row * (Cell_Height + Cell_Gap);

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
        for (i = 1; i <= Animation_Cells * Frames_Per_Cell; i++)
        {
            UpdateAnimations();
            DrawScreen();
        }
    }
}
