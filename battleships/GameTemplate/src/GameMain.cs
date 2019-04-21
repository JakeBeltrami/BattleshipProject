using System;
using SwinGameSDK;
using static SwinGameSDK.SwinGame; // requires mcs version 4+, 
// using SwinGameSDK.SwinGame; // requires mcs version 4+, 

namespace MyGame
{
    public class GameMain
    {
        public static void Main()
        {
            //Open the game window
            OpenGraphicsWindow("GameMain", 800, 600);
            //ShowSwinGameSplashScreen();
            
            GameResources.LoadResources();
            PlayMusic(GameResources.GameMusic("Background"));

            //Run the game loop
            while (false == WindowCloseRequested())
            {
               
                GameController.HandleUserInput();
                GameController.DrawScreen();
            }
        }
    }
}
