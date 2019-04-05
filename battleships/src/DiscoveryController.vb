Using System;
Using System.Collections.Generic;
Using System.Diagnostics;
Using System.Globalization;
Using System.IO;
Using System.Linq;
Using System.Reflection;
Using System.Runtime.CompilerServices;
Using System.Security;
Using System.Text;
Using System.Threading.Tasks;
Using Microsoft.VisualBasic;
Using SwinGameSDK;

/// <summary>

/// ''' The battle phase is handled by the DiscoveryController.

/// ''' </summary>
Static Class DiscoveryController
{

    /// <summary>
    ///     ''' Handles input during the discovery phase of the game.
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Escape opens the game menu. Clicking the mouse will
    ///     ''' attack a location.
    ///     ''' </remarks>
    Public Static void HandleDiscoveryInput()
    {
        If (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
            AddNewState(GameState.ViewingGameMenu);

        If (SwinGame.MouseClicked(MouseButton.LeftButton))
            DoAttack();
    }

    /// <summary>
    ///     ''' Attack the location that the mouse if over.
    ///     ''' </summary>
    Private Static void DoAttack()
    {
        Point2D mouse;

        mouse = SwinGame.MousePosition();

        // Calculate the row/col clicked
        int row, col;
        row = Convert.ToInt32(Math.Floor((mouse.Y - FIELD_TOP) / (double)(CELL_HEIGHT + CELL_GAP)));
        col = Convert.ToInt32(Math.Floor((mouse.X - FIELD_LEFT) / (double)(CELL_WIDTH + CELL_GAP)));

        If (row >= 0 & row < HumanPlayer.EnemyGrid.Height)
        {
            If (col >= 0 & col < HumanPlayer.EnemyGrid.Width)
                Attack(row, col);
        }
    }

    /// <summary>
    ///     ''' Draws the game during the attack phase.
    ///     ''' </summary>s
    Public Static void DrawDiscovery()
    {
        Const int SCORES_LEFT = 172;
        Const int SHOTS_TOP = 157;
        Const int HITS_TOP = 206;
        Const int SPLASH_TOP = 256;

        If ((SwinGame.KeyDown(KeyCode.VK_LSHIFT) | SwinGame.KeyDown(KeyCode.VK_RSHIFT)) & SwinGame.KeyDown(KeyCode.VK_C))
            DrawField(HumanPlayer.EnemyGrid, ComputerPlayer, true);
        Else
            DrawField(HumanPlayer.EnemyGrid, ComputerPlayer, false);

        DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer);
        DrawMessage();

        SwinGame.DrawText(HumanPlayer.Shots.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
        SwinGame.DrawText(HumanPlayer.Hits.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, HITS_TOP);
        SwinGame.DrawText(HumanPlayer.Missed.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
    }
}
