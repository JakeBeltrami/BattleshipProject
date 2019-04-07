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
using SwinGameSDK;

/// <summary>

/// The DeploymentController controls the players actions

/// ''' during the deployment phase.

/// </summary>
static class DeploymentController
{
    private const static int ShipsTop = 98;
    private const static int ShipsLeft = 20;
    private const static int ShipsHeight = 90;
    private const static int ShipsWide = 300;

    private const static int TopButtonTop = 72;
    private const static int TopButtonsHeight = 46;

    private const static int PlayButtonLeft = 693;
    private const static int PlayButtonWidth = 80;

    private const static int UpDownButtonLeft= 410;
    private const static int LeftRightButtonLeft= 350;

    private const static int RandomButtonLeft= 547;
    private const static int RandomButtonWidth= 51;

    private const static int DirButtonsWidth = 47;

    private const static int TextOffset = 5;

    private static DirectionCurrentDirection = Direction.UpDown;
    private static ShipNameSelectedShip = ShipName.Tug;

    /* <summary>
         Handles user input for the Deployment phase of the game.
     </summary>
    <remarks>
         Involves selecting the ships, deloying ships, changing the direction
        of the ships to add, randomising deployment, end then ending
         deployment
     </remarks>*/
    public static void HandleDeploymentInput()
    {
        if (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
            AddNewState(GameState.ViewingGameMenu);

        if (SwinGame.KeyTyped(KeyCode.VK_UP) | SwinGame.KeyTyped(KeyCode.VK_DOWN))
            currentDirection = Direction.UpDown;
        if (SwinGame.KeyTyped(KeyCode.VK_LEFT) | SwinGame.KeyTyped(KeyCode.VK_RIGHT))
            currentDirection = Direction.LeftRight;

        if (SwinGame.KeyTyped(KeyCode.VK_R))
            HumanPlayer.RandomizeDeployment();

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
        {
            ShipName selected;
            selected = GetShipMouseIsOver();
            if (selected != ShipName.None)
                selectedShip = selected;
            else
                DoDeployClick();

            if (HumanPlayer.ReadyToDeploy & IsMouseInRectangle(PlayButtonLeft, TopButtonTop, PlayButtonWidth, TopButtonsHeight))
                EndDeployment();
            else if (IsMouseInRectangle(UpDownButtonLeft, TopButtonTop, DirButtonsWidth, TopButtonsHeight))
                currentDirection = Direction.LeftRight;
            else if (IsMouseInRectangle(LeftRightButtonLeft, TopButtonTop, DirButtonsWidth,TopButtonsHeight))
                currentDirection = Direction.LeftRight;
            else if (IsMouseInRectangle(RandomButtonLeft, TopButtonTop, RandomButtonWidth, TopButtonsHeight))
                HumanPlayer.RandomizeDeployment();
        }
    }

    /* <summary>
     The user has clicked somewhere on the screen, check if its is a deployment and deploy
     the current ship if that is the case.
    </summary>
   <remarks>
     If the click is in the grid it deploys to the selected location
     with the indicated direction
        </remarks>
        */
    private static void DoDeployClick()
    {
        Point2D mouse;

        mouse = SwinGame.MousePosition();

        // Calculate the row/col clicked
        int row, col;
        row = Convert.ToInt32(Math.Floor((mouse.Y) / (double)(CellHeight + CellGap)));
        col = Convert.ToInt32(Math.Floor((mouse.X - FieldLeft) / (double)(cellWidth + CellGap)));

        if (row >= 0 & row < HumanPlayer.PlayerGrid.Height)
        {
            if (col >= 0 & col < HumanPlayer.PlayerGrid.Width)
            {
                // if in the area try to deploy
                try
                {
                    HumanPlayer.PlayerGrid.MoveShip(row, col, SelectedShip, CurrentDirection);
                }
                catch (Exception ex)
                {
                    Audio.PlaySoundEffect(GameSound("Error"));
                    Message = ex.Message;
                }
            }
        }
    }

    /// <summary>
    ///     ''' Draws the deployment screen showing the field and the ships
    ///     ''' that the player can deploy.
    ///     ''' </summary>
    public static void DrawDeployment()
    {
        DrawField(HumanPlayer.PlayerGrid, HumanPlayer, true);

        // Draw the Left/Right and Up/Down buttons
        if (CurrentDirection == Direction.LeftRight)
            SwinGame.DrawBitmap(GameImage("LeftRightButton"), LeftRightButtonLeft, TopButtonTop);
        else
            SwinGame.DrawBitmap(GameImage("UpDownButton"), LeftRightButtonLeft,TopButtonTop);

        // DrawShips
        foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
        {
            int i;
            i = Int(sn) - 1;
            if (i >= 0)
            {
                if (sn == SelectedShip)
                    SwinGame.DrawBitmap(GameImage("SelectedShip"), ShipsLeft, ShipsTop + i * ShipsHeight);
            }
        }

        if (HumanPlayer.ReadyToDeploy)
            SwinGame.DrawBitmap(GameImage("PlayButton"), PlayButtonLeft, TopButtonTop);

        SwinGame.DrawBitmap(GameImage("RandomButton"), RandomButtonLeft, TopButtonTop);

        DrawMessage();
    }

    /// <summary>
    ///     ''' Gets the ship that the mouse is currently over in the selection panel.
    ///     ''' </summary>
    ///     ''' <returns>The ship selected or none</returns>
    private static ShipName GetShipMouseIsOver()
    {
        foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
        {
            int i;
            i = Int(sn) - 1;

            if (IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * ShipsHeight, ShipsWide, ShipsHeight))
                return sn;
        }

        return ShipName.None;
    }
}

