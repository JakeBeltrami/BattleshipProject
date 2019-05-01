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

/// The menu controller handles the drawing and user interactions
/// from the menus in the game. These include the main menu, game
/// menu and the settings menu.
/// </summary>

static class MenuController
{

    /// <summary>
    /// The menu structure for the game.
    /// </summary>
    /// <remarks>
    /// These are the text captions for the menu items.
    /// </remarks>
    private readonly static string[][] _menuStructure = new[] { new string[] { "PLAY", "SETUP", "SCORES", "QUIT" }, new string[] { "RETURN", "SURRENDER", "QUIT" }, new string[] { "EASY", "MEDIUM", "HARD" } };

    private const int Menu_Top = 575;
    private const int Menu_Left = 30;
    private const int Menu_Gap = 0;
    private const int Button_Width = 75;
    private const int Button_Height = 15;
    private const int Button_Sep = Button_Width + Menu_Gap;
    private const int Text_Offset = 0;

    private const int Main_Menu = 0;
    private const int Game_Menu = 1;
    private const int Setup_Menu = 2;

    private const int Main_Menu_Play_Button = 0;
    private const int Main_Menu_Setup_Button = 1;
    private const int Main_Menu_Top_Scores_Button = 2;
    private const int Main_Menu_Quit_Button = 3;

    private const int Setup_Menu_Easy_Button = 0;
    private const int Setup_Menu_Medium_Button = 1;
    private const int Setup_Menu_Hard_Button = 2;
    private const int Setup_Menu_Exit_Button = 3;

    private const int Game_Menu_Return_Button = 0;
    private const int Game_Menu_Surrender_Button = 1;
    private const int Game_Menu_Quit_Button = 2;

    private readonly static Color Menu_Colour = SwinGame.RGBAColor(2, 167, 252, 255);
    private readonly static Color Highlight_Colour = SwinGame.RGBAColor(1, 57, 86, 255);

    /// <summary>
    /// Handles the processing of user input when the main menu is showing
    /// </summary>
    public static void HandleMainMenuInput()
    {
        HandleMenuInput(Main_Menu, 0, 0);
    }

    /// <summary>
    /// Handles the processing of user input when the main menu is showing
    /// </summary>
    public static void HandleSetupMenuInput()
    {
        bool handled;
        handled = HandleMenuInput(Setup_Menu, 1, 1);

        if (!handled)
            HandleMenuInput(Main_Menu, 0, 0);
    }

    /// <summary>
    /// Handle input in the game menu.
    /// </summary>
    /// <remarks>
    /// Player can return to the game, surrender, or quit entirely
    /// </remarks>
    public static void HandleGameMenuInput()
    {
        HandleMenuInput(Game_Menu, 0, 0);
    }

    /// <summary>
    /// Handles input for the specified menu.
    /// </summary>
    /// <param name="menu">the identifier of the menu being processed</param>
    /// <param name="level">the vertical level of the menu</param>
    /// <param name="xOffset">the xoffset of the menu</param>
    /// <returns>false if a clicked missed the buttons. This can be used to check prior menus.</returns>
    private static bool HandleMenuInput(int menu, int level, int xOffset)
    {
        if (SwinGame.KeyTyped(KeyCode.EscapeKey))
        {
            GameController.EndCurrentState();
            return true;
        }

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
        {
            int i;
            for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
            {
                // IsMouseOver the i'th button of the menu
                if (IsMouseOverMenu(i, level, xOffset))
                {
                    PerformMenuAction(menu, i);
                    return true;
                }
            }

            if (level > 0)
                // none clicked - so end this sub menu
                GameController.EndCurrentState();
        }

        return false;
    }

    /// <summary>
    /// Draws the main menu to the screen.
    /// </summary>
    public static void DrawMainMenu()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Main Menu", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(Main_Menu);
    }

    /// <summary>
    /// Draws the Game menu to the screen
    /// </summary>
    public static void DrawGameMenu()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Paused", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(Game_Menu);
    }

    /// <summary>
    /// Draws the settings menu to the screen.
    /// </summary>
    /// <remarks>
    /// Also shows the main menu
    /// </remarks>
    public static void DrawSettings()
    {
        // Clears the Screen to Black
        // SwinGame.DrawText("Settings", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(Main_Menu);
        DrawButtons(Setup_Menu, 1, 1);
    }

    /// <summary>
    /// Draw the buttons associated with a top level menu.
    /// </summary>
    /// <param name="menu">the index of the menu to draw</param>
    private static void DrawButtons(int menu)
    {
        DrawButtons(menu, 0, 0);
    }

    /// <summary>
    /// Draws the menu at the indicated level.
    /// </summary>
    /// <param name="menu">the menu to draw</param>
    /// <param name="level">the level (height) of the menu</param>
    /// <param name="xOffset">the offset of the menu</param>
    /// <remarks>
    /// The menu text comes from the _menuStructure field. The level indicates the height
    /// of the menu, to enable sub menus. The xOffset repositions the menu horizontally
    /// to allow the submenus to be positioned correctly.
    /// </remarks>
    private static void DrawButtons(int menu, int level, int xOffset)
    {
        int btnTop;
        Rectangle toDraw = new Rectangle();

        btnTop = Menu_Top - (Menu_Gap + Button_Height) * level;
        int i;
        for (i = 0; i <= _menuStructure[menu].Length - 1; i++)
        {
            int btnLeft = Menu_Left + Button_Sep * (i + xOffset);

            // SwinGame.FillRectangle(Color.White, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
            toDraw.X = btnLeft + Text_Offset;
            toDraw.Y = btnTop + Text_Offset;
            toDraw.Width = Button_Width;
            toDraw.Height = Button_Height;
            SwinGame.DrawText(_menuStructure[menu][i], Menu_Colour, Color.Black, GameResources.GameFont("Menu"), FontAlignment.AlignCenter, toDraw);

            if (SwinGame.MouseDown(MouseButton.LeftButton) & IsMouseOverMenu(i, level, xOffset))
                SwinGame.DrawRectangle(Highlight_Colour, btnLeft, btnTop, Button_Width, Button_Height);
        }
    }

    /// <summary>
    /// Determined if the mouse is over one of the button in the main menu.
    /// </summary>
    /// <param name="button">the index of the button to check</param>
    /// <returns>true if the mouse is over that button</returns>
    private static bool IsMouseOverButton(int button)
    {
        return IsMouseOverMenu(button, 0, 0);
    }

    /// <summary>
    /// Checks if the mouse is over one of the buttons in a menu.
    /// </summary>
    /// <param name="button">the index of the button to check</param>
    /// <param name="level">the level of the menu</param>
    /// <param name="xOffset">the xOffset of the menu</param>
    /// <returns>true if the mouse is over the button</returns>
    private static bool IsMouseOverMenu(int button, int level, int xOffset)
    {
        int btnTop = Menu_Top - (Menu_Gap + Button_Height) * level;
        int btnLeft = Menu_Left + Button_Sep * (button + xOffset);

        return UtilityFunctions.IsMouseInRectangle(btnLeft, btnTop, Button_Width, Button_Height);
    }

    /// <summary>
    /// A button has been clicked, perform the associated action.
    /// </summary>
    /// <param name="menu">the menu that has been clicked</param>
    /// <param name="button">the index of the button that was clicked</param>
    private static void PerformMenuAction(int menu, int button)
    {
        switch (menu)
        {
            case Main_Menu:
                {
                    PerformMainMenuAction(button);
                    break;
                }

            case Setup_Menu:
                {
                    PerformSetupMenuAction(button);
                    break;
                }

            case Game_Menu:
                {
                    PerformGameMenuAction(button);
                    break;
                }
        }
    }

    /// <summary>
    /// The main menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformMainMenuAction(int button)
    {
        switch (button)
        {
            case Main_Menu_Play_Button:
                {
                    GameController.StartGame();
                    break;
                }

            case Main_Menu_Setup_Button:
                {
                    GameController.AddNewState(GameState.AlteringSettings);
                    break;
                }

            case Main_Menu_Top_Scores_Button:
                {
                    GameController.AddNewState(GameState.ViewingHighScores);
                    break;
                }

            case Main_Menu_Quit_Button:
                {
                    GameController.EndCurrentState();
                    break;
                }
        }
    }

    /// <summary>
    /// The setup menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformSetupMenuAction(int button)
    {
        switch (button)
        {
            case Setup_Menu_Easy_Button:
                {
                    // improved // change to easy 
                    GameController.SetDifficulty(AIOption.Easy);
                    break;
                }

            case Setup_Menu_Medium_Button:
                {
                    // change to medium // improved
                    GameController.SetDifficulty(AIOption.Medium);
                    break;
                }

            case Setup_Menu_Hard_Button:
                {
                    GameController.SetDifficulty(AIOption.Hard);
                    break;
                }
        }
        // Always end state - handles exit button as well
        GameController.EndCurrentState();
    }

    /// <summary>
    /// The game menu was clicked, perform the button's action.
    /// </summary>
    /// <param name="button">the button pressed</param>
    private static void PerformGameMenuAction(int button)
    {
        switch (button)
        {
            case Game_Menu_Return_Button:
                {
                    GameController.EndCurrentState();
                    break;
                }

            case Game_Menu_Surrender_Button:
                {
                    GameController.EndCurrentState(); // end game menu
                    GameController.EndCurrentState(); // end game
                    break;
                }

            case Game_Menu_Quit_Button:
                {
                    GameController.AddNewState(GameState.Quitting);
                    break;
                }
        }
    }
}
