using System;
using SwinGameSDK;

/// <summary>

///The battle phase is handled by the DiscoveryController.

///</summary>
static class DiscoveryController
{

    /// <summary>
    ///Handles input during the discovery phase of the game.
    ///</summary>
    ///<remarks>
    ///Escape opens the game menu. Clicking the mouse will
    ///attack a location.
    ///</remarks>
    public static void HandleDiscoveryInput()
    {
        if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            GameController.AddNewState(GameState.ViewingGameMenu);

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
            DoAttack();
    }

    /// <summary>
    /// Attack the location that the mouse if over.
    /// </summary>
    private static void DoAttack()
    {
        Point2D mouse;

        mouse = SwinGame.MousePosition();

        // Calculate the row/col clicked
        int row, col;
        row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.Field_Top) / (double)(UtilityFunctions.Cell_Height + UtilityFunctions.Cell_Gap)));
        col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.Field_Left) / (double)(UtilityFunctions.Cell_Height + UtilityFunctions.Cell_Gap)));

        if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height)
        {
            if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width)
                GameController.Attack(row, col);
        }
    }

    /// <summary>
    /// Draws the game during the attack phase.
    /// </summary>s
    public static void DrawDiscovery()
    {
        const int SCORES_LEFT = 172;
        const int SHOTS_TOP = 157;
        const int HITS_TOP = 206;
        const int SPLASH_TOP = 256;

        if ((SwinGame.KeyDown(KeyCode.EscapeKey) | SwinGame.KeyDown(KeyCode.RightShiftKey)) & SwinGame.KeyDown(KeyCode.CKey))
            UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, true);
        else
            UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, false);

        UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);
        UtilityFunctions.DrawMessage();

        SwinGame.DrawText(GameController.HumanPlayer.Shots.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
        SwinGame.DrawText(GameController.HumanPlayer.Hits.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
        SwinGame.DrawText(GameController.HumanPlayer.Missed.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
    }
}
