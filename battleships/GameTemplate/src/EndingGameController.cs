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


 //The EndingGameController is responsible for managing the interactions at the end  of a game.


static class EndingGameController
{

    
   //Draw the end of the game screen, shows the win/lose state
    
    public static void DrawEndOfGame()
    {
        Rectangle toDraw;
        string whatShouldIPrint;

        DrawField(ComputerPlayer.PlayerGrid, ComputerPlayer, true);
        DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer);

        toDraw.X = 0;
        toDraw.Y = 250;
        toDraw.Width = SwinGame.ScreenWidth();
        toDraw.Height = SwinGame.ScreenHeight();

        if (HumanPlayer.IsDestroyed)
            whatShouldIPrint = "YOU LOSE!";
        else
            whatShouldIPrint = "-- WINNER --";

        SwinGame.DrawTextLines(whatShouldIPrint, Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, toDraw);
    }


 // Handle the input during the end of the game. Any interaction will result in it reading in the highsSwinGame.
 
    public static void HandleEndOfGameInput()
    {
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.VK_RETURN) || SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
        {
            ReadHighScore(HumanPlayer.Score);
            EndCurrentState();
        }
    }
}
